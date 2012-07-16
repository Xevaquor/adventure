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
    public delegate void RequestStateChangeDelegate(IGameState sender, string requested_state);
    public delegate void RequestContent(IGameState sender, IEnumerable<ResMan.Asset> assets);

    public interface IGameState
    {
        void Draw(SpriteBatch batch);
        void Update();
        void Activate(object obj);

        event RequestStateChangeDelegate StateChangeRequested;
        event RequestContent ContentRequested;
    }
}
