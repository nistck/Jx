using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
using Jx.Engine;
using Jx.Engine.Game;
using Jx.Engine.System;
using Jx.Engine.Entity;
using Jx.Engine.Collections;

namespace JxECS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public MyGameManager GM { get; private set; }
        public GravitySystem Gv { get; private set; }
        public PlayerSystem Ps { get; private set; }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GM?.Update(new TickEvent());
        }

        private bool IsGravityEntity(IEntity entity)
        {
            return entity != null && entity.HasComponents(typeof(Movable), typeof(Position));
        }

        private bool IsPlayerEntity(IEntity entity)
        {
            return entity != null && entity.HasComponents(typeof(Movable), typeof(Position), typeof(HasLife));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\n--- \nGM: {0}", GM.ID);

            if (Gv != null)
            {
                foreach (var entity in Gv)
                {
                    Position p = entity.GetComponent<Position>(); 
                    Console.WriteLine("Gv: {0}, Position: ({1}, {2})", entity, p.X, p.Y);
                }
            }

            if( Ps != null)
            {
                foreach(var entity in Ps)
                {
                    HasLife Lx = entity.GetComponent<HasLife>();
                    Console.WriteLine("Ps: {0}, {1}, Owner: {2}", entity, Lx.Health, Lx.Owner);
                }
            }

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GM = MyGameManager.New();

            Ps = new PlayerSystem(IsPlayerEntity);
            GM.AddSystem(Ps);

            Gv = new GravitySystem(IsGravityEntity);
            
            for (int i = 0; i < 2; i++)
            {
                string hero_name = string.Format("Ov-{0}", i);
                IEntity hero = GM.CreateEntity(hero_name);
                hero.AddComponent(new Movable());
                hero.AddComponent(new Position());
                hero.AddComponent(new HasLife()); 
            }

            IEntity rock = GM.CreateEntity("RedRock");
            rock.AddComponent(new Movable());
            rock.AddComponent(new Position()); 

            GM.AddSystem(Gv); 
        }
    }

    public class TickEvent : ITickEvent
    {
        public TimeSpan ElapsedGameTime { get; set; }
        public bool IsRunningSlowly { get; set; }
        public TimeSpan TotalGameTime { get; set; }
    }
}
