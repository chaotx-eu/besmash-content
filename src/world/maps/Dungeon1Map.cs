namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class Dungeon1Map : TileMap {
        public Dungeon1Map() : base() {
            // nothing special required here
        }

        public override void init(Game game) {
            base.init(game);

            // define special tiles (doors, triggers, etc.)
            Tile cryptExit1 = getTile(49, 96);
            Tile stairsDown = getTile(75, 8);
            cryptExit1.TileSteppedEvent += (sender, args) => {
                // define here what should happen when stepped on this tile
                if(args.Movable == Slave)
                    loadOther("maps/forestMap"); // e.g. load another map (map file must exist!)
            };
            stairsDown.TileSteppedEvent += (sender, args) => {
                // define here what should happen when stepped on this tile
                if(args.Movable == Slave)
                    loadOther("maps/dungeon2"); // e.g. load another map (map file must exist!)
            };
        }

        public override void onLoad(TileMap fromMap, Team team) {
            if(fromMap is ForestMap) {
                team.Player.ForEach(player
                    => player.Position = new Vector2(49, 96));
            }
            else if(fromMap is Dungeon2Map) {
                team.Player.ForEach(player
                    => player.Position = new Vector2(75, 8));
            }
            // always call this last
            base.onLoad(fromMap, team);
        }

        /// Override this method to spawn and add
        /// entities on load
        protected override void spawnEntities() {
            base.spawnEntities(); // clears entity list by default
            Random rand = new Random();
            int maxNumber=30;
            Point p1;
            p1.X=1;
            p1.Y=1;
            Point p2;
            p2.X=99;
            p2.Y=99;
            int i=0;
            while(i<maxNumber){
                int posL1=rand.Next(p1.X, p2.X);
                int posL2=rand.Next(p1.Y, p2.Y);
                if(getTile(posL1,posL2).Solid==false&&getEntities(posL1,posL2).Count==0){
                    Group group = new Group();
                    Enemy leader = new Enemy();
                    leader.SpriteSheet = "images/entities/kevin_sheet";
                    leader.SpriteRectangle = new Rectangle(0, 0, 16, 16);
                    int direction = rand.Next(1,4);
                    switch (direction)
                    {
                        case 1:
                            leader.Facing = Facing.NORTH;
                            break;
                        case 2:
                            leader.Facing = Facing.EAST;
                            break;
                        case 3:
                            leader.Facing = Facing.SOUTH;
                            break;
                        case 4:
                            leader.Facing = Facing.WEST;
                            break;
                        default:
                            leader.Facing = Facing.SOUTH;
                            break;
                    }                 
                    leader.Position = new Vector2(posL1,posL2);
                    addEntity(leader);
                    group.adMember(leader);
                    leader.group=group;
                    int groupSize=rand.Next(1, 4);
                    int j=0;
                    int maxTry=0;
                    while(j<groupSize&&maxTry<10){
                        int posM1=rand.Next(posL1-2, posL1+2);
                        int posM2=rand.Next(posL2-2, posL2+2);
                        if(posM1>=p1.X && posM1<=p2.X && posM2>=p1.Y && posM2<=p2.Y && getTile(posM1,posM2).Solid==false&&leader.canSee(posM1,posM2)&&getEntities(posM1,posM2).Count==0){
                            Enemy minion = new Enemy();
                            minion.SpriteSheet = "images/entities/kevin_sheet";
                            minion.SpriteRectangle = new Rectangle(0, 0, 16, 16);
                            direction = rand.Next(1,4);
                            switch (direction)
                            {
                                case 1:
                                    minion.Facing = Facing.NORTH;
                                    break;
                                case 2:
                                    minion.Facing = Facing.EAST;
                                    break;
                                case 3:
                                    minion.Facing = Facing.SOUTH;
                                    break;
                                case 4:
                                    minion.Facing = Facing.WEST;
                                    break;
                                default:
                                    minion.Facing = Facing.SOUTH;
                                    break;
                            }  
                            minion.Position = new Vector2(posM1,posM2);
                            addEntity(minion);  
                            group.adMember(minion);
                            minion.group=group;
                            j++;                    
                        }
                        maxTry++;
                    }
                    i++;
                }
            }
            
        }
        
    }
}