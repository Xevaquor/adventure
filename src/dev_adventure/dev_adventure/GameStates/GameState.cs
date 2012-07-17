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
using System.Diagnostics;

namespace dev_adventure
{
    public delegate void RequestStateChangeDelegate(GameState sender, string requested_state);
    public delegate void RequestContent(GameState sender, IEnumerable<ResMan.Asset> assets);

    public abstract class GameState
    {
        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected HashSet<ResMan.Asset> requiredResources = new HashSet<ResMan.Asset>();
        protected bool initialized = false;

        public SpriteBatch spriteBatch;

        public abstract void Draw();
        public abstract void Update();
        public virtual void Activate(object obj) 
        { 
            /*
             * if call HandleResources
             *      call AssignResources
             */
        }

        protected bool HandleResources()
        {
            if (ResMan.ResourcesLoaded(requiredResources))
                return true;
            else
            {
                if (ContentRequested != null)
                {
                    ContentRequested(this, requiredResources);
                }
                else
                    logger.Warn("ContentRequest event is not handled");
            }
            return false;
        }

        /*
         * -AssignResources : void
         *      tex = ResMan.GetResource<T>(name);
         *      ...
         */      
        
        protected virtual void SetRequiredResources() { }
        
        protected void RaiseStateChangeRequest(string requested_state)
        {
            if (StateChangeRequested != null)
                StateChangeRequested(this, requested_state);
            else
                logger.Warn("StateChangeRequest is not handled");
        }

        protected void RaiseContentRequest()
        {
            if (ContentRequested != null)
                ContentRequested(this, requiredResources);
            else
                logger.Warn("ContentRequest is not handled");
        }

        public event RequestStateChangeDelegate StateChangeRequested;
        public event RequestContent ContentRequested;
    }
}
