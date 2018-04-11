using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.BT
{
    public abstract class BTNodeType : EntityType
    {
        [FieldSerialize]
        private BTContextType _context;

        [DisplayName("环境")]
        public BTContextType Context
        {
            get { return _context; }
            set { this._context = value; }
        }

    }

    public abstract class BTNode : Entity
    {
        private BTNodeType _type; 
        public new BTNodeType Type { get { return _type; } }

        [FieldSerialize]
        private BTContext _context;
        public BTContext Context
        {
            get { return _context; }
            set { this._context = value; }
        }


        private List<BTConstraint> m_Constraints = new List<BTConstraint>(); 

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; protected set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name_ { get; protected set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; protected set; }

        /// <summary>
        /// 覆盖返回结果
        /// </summary>
        public BTResult MuteResult { get; set; } = null;

        /// <summary>
        /// 运行中? 
        /// </summary>
        public bool Running { get; protected set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public BTNode Parent_ { get; internal set; }

        public BTResult Result { get; private set; }

        public BTNode()
        {
            this.Id = Guid.NewGuid().ToString(); 
        }

        protected override void OnCreate()
        {
            base.OnCreate(); 

            if( Type.Context != null )
                Context = Entities.Instance.Create(Type.Context, Parent) as BTContext;
        }

        protected override void OnPostCreate(bool loaded)
        {
            base.OnPostCreate(loaded);

            if( Parent_ == null )
                SubscribeToTickEvent(); 
        }

        protected override void OnDestroy()
        {
            if( Parent_ == null )
                UnsubscribeToTickEvent(); 

            base.OnDestroy();
        }

        public bool AddConstraint(BTConstraint constraint)
        {
            if (constraint == null || m_Constraints.Contains(constraint))
                return false;

            m_Constraints.Add(constraint);
            return true;
        }

        public bool RemoveConstraint(BTConstraint constraint)
        {
            if (constraint == null || !m_Constraints.Contains(constraint))
                return false;

            bool b = m_Constraints.Remove(constraint);
            return b;
        }

        public bool RemoveConstraint(string constraintId)
        {
            if (constraintId == null)
                return false;

            BTConstraint c = m_Constraints.Where(_c => _c.Id == constraintId).FirstOrDefault();
            return RemoveConstraint(c); 
        }

        public List<BTConstraint> Constraints
        {
            get { return m_Constraints.ToList(); }
        }

        public int ConstraintCount
        {
            get { return m_Constraints.Count; }
        }

        private BTResult _MuteResult(BTResult r)
        {
            return MuteResult == null ? r : MuteResult;
        }

        protected override void OnTick()
        {
            base.OnTick();
            Result = Tick_(Context); 
        }

        /// <summary>
        /// 每帧处理函数
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        internal BTResult Tick_(BTContext context)
        {
            context?.Travel(this); 

            BTResult result = BTResult.Running;

            bool constraintsTrue = CheckConstraints(context);
            if (!constraintsTrue)
            {
                result = _MuteResult(BTResult.Failed);
            }
            else
            {
                if (!_sessionOpen)
                    _open(context);
                 
                _enter(context);
                try
                {
                    result = OnTick(context);
                }
                finally
                {
                    _exit(context, result);
                }

                if( result != BTResult.Running )
                { 
                    _close(context, result); 
                }
            }
             
            context?.SetNodeResult(this, result); 
            return _MuteResult(result); 
        } 

        private void _exit(BTContext context, BTResult result)
        {
            OnExit(context, result); 
        }

        private void _enter(BTContext context)
        {
            OnEnter(context); 
        }

        private void _open(BTContext context)
        {
            _sessionOpen = true;
            OnOpen(context); 
        }

        private void _close(BTContext context, BTResult result)
        {
            _sessionOpen = false;
            OnClose(context, result);
        }

        #region 临时变量
        private bool _sessionOpen = false; 
        #endregion

        /// <summary>
        /// 检查前置条件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool CheckConstraints(BTContext context)
        {
            bool result = true;  
            m_Constraints.RemoveAll(_x => _x == null); 
            for(int i = 0; i < m_Constraints.Count; i ++)
            {
                bool b = m_Constraints[i].Execute(context); 
                if( !b )
                {
                    result = false;
                    break; 
                }
            }

            return result;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            Running = false;
        } 

        /// <summary>
        /// 开启新Session （在Running时，不会开启新Session）
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnOpen(BTContext context) { }

        /// <summary>
        /// Session结束
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        protected virtual void OnClose(BTContext context, BTResult result) { }

        /// <summary>
        /// 执行之前
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnEnter(BTContext context) { }

        /// <summary>
        /// 执行之后
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        protected virtual void OnExit(BTContext context, BTResult result) { }

        /// <summary>
        /// 执行逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual BTResult OnTick(BTContext context) { return BTResult.Failed; }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnLoad(BTContext context) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnInit(BTContext context) { }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnSave(BTContext context) { } 

    }
}
