using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Jx
{
    public class JxThread
    {
        private static readonly object jxLock = new object();
        private static int jxId = 1;

        class WorkerInfo
        {
            private long time = 0; 

            private Action<object> workerProc = null;
            private readonly List<object> items = new List<object>();
            private readonly ManualResetEventSlim itemEvent = new ManualResetEventSlim(false);

            public WorkerInfo(string name, Action<object> workerProc)
            {
                this.Name = name ?? "Worker";
                this.workerProc = workerProc;
            }

            public string Name { get; private set; }

            public Thread Worker { get; private set; } 
            public bool Running { get; private set; }
            public bool Idle { get; private set; }
            public object Current { get; private set; }
            private bool _CurrentValid;

            public void Quit()
            {
                lock (this)
                {
                    if (Worker != null)
                    {
                        Worker.Interrupt();
                        Worker.Abort();
                        Worker = null;
                    }
                }
            }

            public void Start()
            {
                lock(this)
                {
                    _Start();
                }
            }

            private void _Start()
            {
                if (Worker != null && !Running)
                {
                    Running = true;
                    Worker.Start();
                }
            }

            public void Create(bool startImmediate = true)
            {
                lock(this)
                {
                    if( Worker == null)
                    {
                        Worker = new Thread(new ThreadStart(WorkerProc));
                        Worker.IsBackground = true;
                        Worker.Name = this.Name;
                        this.Idle = true;
                        if (startImmediate)
                            _Start();
                    }
                }
            }

            public void Work(object item)
            {
                lock(items)
                {
                    items.Add(item);
                    itemEvent.Set();
                }
            }

            public int ItemsCount
            {
                get { lock (items) return items.Count; }
            }

            private void WorkerProc()
            {
                while(Running)
                {
                    object item = null;
                    bool itemFound = false; 
                    lock(items)
                    {
                        if( items.Count > 0)
                        {
                            item = items[0];
                            items.RemoveAt(0);
                            itemFound = true;
                        }
                    }

                    if( !itemFound )
                    {
#if DEBUG_THREAD
                        Log.Info(">> [{0}] 工作队列中没有数据， 等待数据...", Thread.CurrentThread.Name);
#endif
                        itemEvent.Wait();
                        itemEvent.Reset();
                        continue; 
                    }

                    try
                    {
                        _SetCurrent(item, true);
                        // 有数据!
                        workQuiet(item);
                    }
                    finally {
                        _SetCurrent(null, false);
                    }
                }
            }

            private void _SetCurrent(object item, bool valid)
            {
                lock(items)
                {
                    Current = item;
                    _CurrentValid = valid;
                }
            }

            private void workQuiet(object item)
            {
#if DEBUG_THREAD
                Log.Info(">> [{0}] 开始处理数据: {1}, 队列长度: {2}", this.Name, item, items.Count);
#endif
                long ts1 = 0; 
                try
                {
                    this.Idle = false;
                    ts1 = DateTime.Now.Ticks;

                    if (workerProc != null)
                        workerProc(item);
                }
                catch (Exception) { }
                finally
                {
                    long ts2 = DateTime.Now.Ticks;
                    this.Idle = true;
                    this.time += (ts2 - ts1) / 10000; 
                }
            }

            /// <summary>
            /// 活动时间，单位： 毫秒
            /// </summary>
            public long Time
            {
                get { return time; }
            }

            public bool ContainsItem(object item)
            {
                lock (items)
                {
                    if (item == null)
                    {
                        int n = items.Where(_x => _x == null).Count();
                        return n > 0 || (_CurrentValid && Current == null);
                    }

                    return items.Contains(item) || (_CurrentValid && Current != null && Current.Equals(item));
                }
            }
        }

        private int id = 0;

        private bool unique = true;
        private int numberOfWorkers = 0;
        private Action<object> itemProcessProc = null;

        private WorkerInfo dispatcher = null;
        private readonly List<WorkerInfo> workers = new List<WorkerInfo>(); 
 
        public JxThread(Action<object> itemProcessProc, int numberOfWorkers = 0, bool unique = true)
        {
            lock (jxLock)
                this.id = jxId++;

            this.itemProcessProc = itemProcessProc;

            numberOfWorkers = numberOfWorkers <= 0 ? (int)(Win32Api.GetNumberOfProcessors() + 2) : numberOfWorkers;
            this.numberOfWorkers = numberOfWorkers; 
        }

        ~JxThread()
        {

        }

        public int Id
        {
            get { return id; }
        }

        public int NumberOfWorkers
        {
            get { return numberOfWorkers; }
        } 
 
        public void Quit()
        {
            lock(this)
            {
                if( this.dispatcher != null )
                {
                    this.dispatcher.Quit();
                    this.dispatcher = null; 
                }

                for(int i = 0; i < workers.Count; i ++)
                {
                    WorkerInfo worker = workers[i];
                    worker.Quit(); 
                }
                workers.Clear();
            }
        }

        public void Work(object item)
        {
            lock(this)
            {
                if( this.dispatcher == null)
                {
                    string name = string.Format("调度线程 #{0}", this.Id);
                    this.dispatcher = new WorkerInfo(name, _ThreadProc);
                    this.dispatcher.Create();

#if DEBUG_THREAD
                    Log.Info(">> [{0}] 创建 {1}", Thread.CurrentThread.Name, name);
#endif
                }

                if (!dispatcher.ContainsItem(item))
                    dispatcher.Work(item);
            }
        }

        private void Work(WorkerInfo worker, object item)
        {
            if (worker == null)
                return;

            if (unique)
            {
                int n = workers.Sum(_worker => _worker.ContainsItem(item) ? 1 : 0);
                if (n == 0)
                    worker.Work(item);
            }
            else
            {
                worker.Work(item);
            }
        }

        /// <summary>
        /// 任务分发线程
        /// </summary>
        /// <param name="item"></param>
        private void _ThreadProc(object item)
        {
            lock(workers)
            {
                if (workers.Count > numberOfWorkers)
                {
                    for (int i = workers.Count - 1; i >= 0; i--)
                    {
                        WorkerInfo worker = workers[i];
                        if (workers.Count > numberOfWorkers && worker.ItemsCount == 0 && worker.Idle)
                        {
                            workers.RemoveAt(i);
                        }
                    }
                }
 
                // 没有Worker空闲
                if(workers.Count == 0 || workers.Count < numberOfWorkers)
                {
                    string workerName = string.Format("Worker #{0}", workers.Count);
                    WorkerInfo worker = new WorkerInfo(workerName, itemProcessProc);
                    workers.Add(worker);
#if DEBUG_THREAD
                    Log.Info(">> [{0}] 创建工作线程 ({1}) 处理数据: {2}", Thread.CurrentThread.Name, workerName, item);
#endif
                    worker.Create();
                    Work(worker, item);
                    return; 
                }

                int itemsMin = workers.Min(_worker => _worker.ItemsCount);
                WorkerInfo workerMinItems = workers.Where(_worker => _worker.ItemsCount == itemsMin).FirstOrDefault();
                if (workerMinItems != null)
                {
#if DEBUG_THREAD
                    Log.Info(">> [{0}] 工作线程({1}) 队列中数据最少 ({2}个), 可以处理数据: {3}", 
                        Thread.CurrentThread.Name, workerMinItems.Name, workerMinItems.ItemsCount, item);
#endif
                    Work(workerMinItems, item);
                    return;
                } 

                WorkerInfo wf = workers[0];
#if DEBUG_THREAD
                bool wfDefault = true;
#endif
                // 负载最轻的 
                long timeMin = workers.Min(_worker => _worker.Time);
                WorkerInfo wx = workers.Where(_worker => _worker.Time == timeMin).FirstOrDefault();
                if (wx != null)
                {
                    wf = wx;
#if DEBUG_THREAD
                    wfDefault = false;
#endif
                }

#if DEBUG_THREAD
                Log.Info(">> [{0}] 使用工作线程 ({1}) 处理数据 {2}, 工作线程队列长度: {3}, 数据: {4}", 
                    Thread.CurrentThread.Name, wf.Name, wfDefault? "(缺省)" : "(负载最轻)", wf.ItemsCount, item);
#endif
                Work(wf, item);
            }
        }

 
    }
}
