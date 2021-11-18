using Sandbox.Game.Entities;
using Sandbox.Game.WorldEnvironment;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace NoTreeCollision
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    class Core : MySessionComponentBase
    {
        List<IMyPlayer> Players = new List<IMyPlayer>();
        List<MyEntity> Entities = new List<MyEntity>();
        BoundingSphereD Bound = new BoundingSphereD(Vector3D.Zero, 500);
        IEnumerator<bool> Cleaner;
        int currentPlayer = 0;
        int count = 0;

        public override void LoadData()
        {
            //MyAPIGateway.Entities.OnEntityAdd += EntityAdded;
            //MyEntities.OnEntityCreate += EntityAdded;
        }

        protected override void UnloadData()
        {
            //MyAPIGateway.Entities.OnEntityAdd -= EntityAdded;
            //MyEntities.OnEntityCreate -= EntityAdded;
        }

        void EntityAdded(IMyEntity ent)
        {
            try
            {
                MyEnvironmentSector tree = ent as MyEnvironmentSector;
                if (tree != null && tree.Physics != null)
                {
                    ent.Physics.Enabled = false;
                    MyLog.Default.WriteLine("TREEEEE! Disable.");
                } else
                {
                    MyLog.Default.WriteLine("NO TREE! Type: " + tree.GetType());
                }
            }
            catch (Exception e)
            {
                MyLog.Default.WriteLine("NoTreeCollision: ERROR" + e.Message);
            }
        }


        public override void UpdateBeforeSimulation()
        {
            try
            {
                if (count++ < 120) return;

                Players.Clear();
                MyAPIGateway.Players.GetPlayers(Players);

                foreach (IMyPlayer p in Players)
                {
                    if (p == null || p.IsBot || p.GetPosition() == null) continue;

                    Bound.Center = p.GetPosition();
                    MyPlanet myPlanet = MyGamePruningStructure.GetClosestPlanet(p.GetPosition());
                    if (myPlanet == null) continue;

                    Entities.Clear();
                    myPlanet.Hierarchy.QuerySphere(ref Bound, Entities);
                    foreach (IMyEntity ent in Entities)
                    {
                        if (!(ent is MyEnvironmentSector) || ent.Physics == null) continue;

                        MyEnvironmentSector tree = ent as MyEnvironmentSector;
                        if (tree == null || !tree.Physics.Enabled) continue;

                        ent.Physics.Enabled = false;
                    }
                }
            }
            catch (Exception e)
            {
                MyLog.Default.WriteLine("NoTreeCollision: ERROR " + e.Message);
            }
        }
    }
}
