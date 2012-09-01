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

namespace DevAdventure
{
    delegate void ResourceRequestDelegate(IEnumerable<ResMan.Asset> res_list);
    delegate void StateChangeRequestDelegate(string name, object obj);

    abstract class IGameState
    {
        protected HashSet<ResMan.Asset> requiredResources = new HashSet<ResMan.Asset>();
        protected Vector2 camera;

        public IGameState()
        {
            //requiredResources.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
        }

        public abstract void Draw(SpriteBatch batch);

        public abstract void Update();

        protected abstract void AssignResources();

        protected bool dirtyResources = true;

        public abstract void Activate(object obj = null);

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

        protected void DrawGameObject(SpriteBatch batch, GameObject obj)
        {
            //if(GameObject
            batch.Draw(obj.Sprite.Sprite, obj.Position - camera , obj.Sprite.Area, Color.White, obj.Rotation, obj.Origin, 1.0f, SpriteEffects.None, 0.0f); 

        }

        protected void LookAt(GameObject target)
        {
            //if(target != null)
                camera = (target.Position + 0*  target.Origin) - Settings.DesiredResolution / 2;
        }

        public event ResourceRequestDelegate RequestingResources;
        public event StateChangeRequestDelegate RequestingStateChange;
    }
}
