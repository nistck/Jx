using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Jx.Editors;
using Jx.EntitiesCommon.Behaviors;

namespace Jx.EntitiesCommon.Editors.Behaviors
{
    public class CompositeNodeType_ChildNodeItemCollectionEditor : GeneralListCollectionEditor
    {
        public CompositeNodeType_ChildNodeItemCollectionEditor()
            : base( typeof(List<CompositeNodeType.ChildNodeItem> ) ) 
        {
        }
    } 
}
