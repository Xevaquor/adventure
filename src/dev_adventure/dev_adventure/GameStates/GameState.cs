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
    public delegate void RequestStateChangeDelegate(IGameState sender, string requested_state, object param);
    public delegate void RequestContent(IGameState sender, IEnumerable<ResMan.Asset> assets);

    public abstract class IGameState
    {
        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected HashSet<ResMan.Asset> requiredResources = new HashSet<ResMan.Asset>();
        protected bool initialized = false;

        public SpriteBatch spriteBatch;

        /// <summary>
        /// Obvious
        /// </summary>
        public abstract void Draw();
        /// <summary>
        /// Obvious.
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// Called after needed resources are loaded. Assigning resources to vars should be put here.
        /// </summary>
        public abstract void Initialize();

        public IGameState()
        {
            SetRequiredResources();
        }
        /// <summary>
        /// Called when state is activated - e.g. every time when player turn pause on.
        /// Ought to call base.Active as shown below:
        /// <code>
        /// if (!base.Activate(obj))
        ///     return false;  //true/false has no meaning. Should *return* from here because state is't ready to execute.
        ///
        /// return true;
        /// </code>
        /// </summary>
        /// <param name="obj">Optional parameter for state, eg list of resources to load. I have no idea what could be other case. 
        /// (Maybe if it would be possible to send argument from state request?</param>
        /// <returns>Return value has meaning only in IGameState - determines if it is reaady to work</returns>
        public virtual bool Activate(object obj) 
        {
            if (HandleResources())
            {
                if (!initialized)
                {
                    Initialize();
                    initialized = true;
                }
                return true;
            }
            return false;
        }

        //TODO: lover access-modifier?
        /// <summary>
        /// Check if needed resources are loaded. If not change state to loading to load it.
        /// Remember to implement SetRequiredResources!
        /// </summary>
        /// <returns>Do we have all needed resources?</returns>
        protected bool HandleResources()
        {
            if (ResMan.ResourcesLoaded(requiredResources))
            {
                return true;
            }
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

        /// <summary>
        /// Set resources what you need. DO NOT LOAD ANY RESOURCES ON YOUR OWN!!!!!!1111oneoneone
        /// </summary>
        protected abstract void SetRequiredResources();
        
        protected void RaiseStateChangeRequest(string requested_state, object param)
        {
            if (StateChangeRequested != null)
                StateChangeRequested(this, requested_state, param);
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
