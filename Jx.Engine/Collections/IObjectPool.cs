namespace Jx.Engine.Collections
{
    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectPool<T>
    {
        /// <summary>
        /// 对象池中对象的数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 返回一个 或者 新创建一个
        /// </summary>
        /// <returns></returns>
        T Get();
        /// <summary>
        /// 放置一个到对象池中
        /// </summary>
        /// <param name="item"></param>
        void Put(T item);
    }
}