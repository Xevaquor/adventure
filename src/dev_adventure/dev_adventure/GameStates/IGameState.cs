using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace dev_adventure
{
    public delegate void ResourceRequestDelegate(IEnumerable<ResMan.Asset> res_list);
    public delegate void StateChangeRequestDelegate(string name, object obj);

    abstract class IGameState
    {
        protected HashSet<ResMan.Asset> requiredResources = new HashSet<ResMan.Asset>();

        public IGameState()
        {
            //requiredResources.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
        }

        public abstract void Draw(SpriteBatch batch);

        public abstract void Update();

        protected abstract void AssignResources();

        protected bool dirtyResources = true;

        public abstract void Activate(object obj);

        protected bool HandleResources()
        {
            if (HaveAllResources())
            {
                if (dirtyResources)
                {
                    AssignResources();
                    dirtyResources = false;
                }
                return true;
            }
            else
            {
                dirtyResources = true;
                //zarządaj nowych zasobów
                RequestingResources(requiredResources);
                //natychmiastowo wyjdź
                return false;
            }
        }

        private bool HaveAllResources()
        {
            return ResMan.ResourcesLoaded(requiredResources);
        }

        protected void RaiseStateChangeRequest(string requested_state, object obj)
        {
            if (RequestingStateChange != null)
                RequestingStateChange(requested_state, obj);
        }

        public event ResourceRequestDelegate RequestingResources;
        public event StateChangeRequestDelegate RequestingStateChange;
    }
}
