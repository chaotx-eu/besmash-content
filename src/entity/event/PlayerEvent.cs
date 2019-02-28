namespace BesmashContent {
    using System;

    public delegate void PlayerEventHandler(Creature sender, PlayerEventArgs args);
    public class PlayerEventArgs : EventArgs {
        public Player Player {get; protected set;}

        public PlayerEventArgs(Player player) {
            Player = player;
        }
    }
}