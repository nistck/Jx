using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem
{
    /// <summary>
    /// Specifies the network type of the entity.
    /// </summary>
    public enum EntityNetworkTypes
    {
        /// <summary>
        /// Entities of this type can be exist on the server and the client, but never will 
        /// be synchronized.
        /// </summary>
        NotSynchronized,
        /// <summary>
        /// Entities of this type exist only on a server.
        /// </summary>
        ServerOnly,
        /// <summary>
        /// Entities of this type exist only on a client.
        /// </summary>
        ClientOnly,
        /// <summary>
        /// Entities of this type exist on the server and the client and it properties 
        /// will be synchronized. Only entities of this type are synchronized between 
        /// the server and clients.
        /// </summary>
        Synchronized
    }
}
