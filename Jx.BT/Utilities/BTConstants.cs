
using System.Collections;

namespace Jx.BT {

    public enum BTMemberType
    {
        Field,
        Property
    }

    public enum BTAbortOpt {
		None,
		Self,
		LowerPriority,
		Both,
	}

	public enum BTClearOpt {
		Default,
		Selected,
		DefaultAndSelected,
		All,
	}

	public enum BTLogic {
		And,
		Or,
	}

	public enum BTExecuteOpt {
		OnTick,
		OnClear,
		Both,
	}

	public enum BTDataReadOpt {
		ReadAtBeginning,
		ReadEveryTick,
	}
}