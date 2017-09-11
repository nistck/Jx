using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitiesCommon;

namespace Jx.Game
{
    public abstract class GameBotType : DynamicType
    {
        public static readonly int MIN_BOT_INTERVAL = 1000;
        [FieldSerialize]
        private int botInterval = MIN_BOT_INTERVAL;

        [Description("Bot执行时间间隔，单位： 毫秒。最低值1000毫秒。")]
        public int BotInterval
        {
            get { return botInterval < MIN_BOT_INTERVAL? MIN_BOT_INTERVAL : botInterval; }
            set { this.botInterval = value; }
        }
    }

    public abstract class GameBot : Dynamic
    {
        private GameBotType _type = null;
        public new GameBotType Type { get { return _type; } }

        [FieldSerialize(FieldSerializeSerializationTypes.World)]
        private int currentInterval = 0;

        protected override void OnPostCreate(bool loaded)
        {
            base.OnPostCreate(loaded);
            SubscribeToTickEvent();
        }

        protected override void OnTick()
        {
            base.OnTick();
            TryRun();
        }

        protected void TryRun()
        {
            currentInterval += JxEngineApp.Instance.LoopInterval;

            if( currentInterval >= Type.BotInterval )
            {
                currentInterval = 0;
                try
                {
                    Run();
                }
                catch (Exception e)
                {
                    Log.Error("GameBot Running Exception: {0}", e.Message);
                }
                finally { }
            }
        }

        protected abstract void Run();
    }
}
