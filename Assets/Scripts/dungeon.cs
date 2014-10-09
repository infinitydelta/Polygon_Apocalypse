using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dungeon : MonoBehaviour {

	public GameObject ground;
	public GameObject wall;
    public GameObject player;
    public GameObject pink;
    public GameObject portal;
    public GameObject enemy;

	public int mapSize = 128;
	Vector2 center;
	int[,] map;
	
	room[] rooms; 
	public int numRooms = 10;
	public int roomSizeMin = 5;
	public int roomSizeMax = 15;

    public int squashTimes = 0;
	// Use this for initialization
	void Start () {

		center = new Vector2(mapSize/2, mapSize/2);
		map =  new int[mapSize,mapSize];
		rooms = new room[numRooms];
        //spawn rooms
		generateDungeon();
        //move rooms together
		squash(squashTimes);
        //connect rooms with hallways
        testConnect();
		createMap();
       
        createTiles();
        Vector3 playerSpawn = getSpawnPosition();
        Instantiate(player, playerSpawn, Quaternion.identity);
        //instantiate portal x units away
        Instantiate(portal, spawnPortal(playerSpawn), Quaternion.identity);
        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(0f, 360f);
            Instantiate(enemy, getSpawnPosition(), Quaternion.Euler(0, 0, angle));
        }
	}
	
	// Update is called once per frame
	void Update () {
       
	}
	
    //spawns rooms randomly
	void generateDungeon() {
		for (int x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++) {
				map[x,y] = 0;
			}
		}
		
		//add new rooms to rooms array
		for (int i = 0; i < numRooms; i++) {
			room newRoom = new room();
            newRoom.index = i;
			newRoom.width = Random.Range(roomSizeMin, roomSizeMax + 1);
			newRoom.height = Random.Range(roomSizeMin, roomSizeMax + 1);
			
			newRoom.x = Random.Range(1 + newRoom.width, mapSize - newRoom.width - 1);
			newRoom.y = Random.Range(1 + newRoom.height, mapSize - newRoom.height - 1);
			newRoom.center = new Vector2(newRoom.x + (newRoom.width)/2, newRoom.y + (newRoom.height)/2);
            newRoom.connected = false;

			if (doesCollide(newRoom)) {
				//remake the room
				i--;
				continue;
			}

			rooms[i] = newRoom;
			
		}
		

	}
	
	bool doesCollide(room room, int ignore = -1) {
		for (int i = 0; i < numRooms; i++) {
			if (i == ignore) {
				continue;
			}
			room check = rooms[i];
			//de morgan's law
			//-1 +1 -1 +1
			if (!((room.x + room.width + 1< check.x ) || (room.x - 1> check.x + check.width ) || (room.y + room.height < check.y ) || (room.y > check.y + check.height))) {
				return true;			
			}
		}
		return false;		
	}

    /*
    bool doesCollideX(room room, int ignore = -1)
    {
        for (int i = 0; i < numRooms; i++)
        {
            if (i == ignore)
            {
                continue;
            }
            room check = rooms[i];
            //de morgan's law
            //-1 +1 -1 +1
            if (!((room.x + room.width + 1 < check.x) || (room.x - 1 > check.x + check.width) ))
            {
                return true;
            }
        }
        return false;
    }
    */
    bool doesCollideY(room room, int ignore = -1)
    {
        for (int i = 0; i < numRooms; i++)
        {
            if (i == ignore)
            {
                continue;
            }
            room check = rooms[i];
            //de morgan's law
            //theres no "wall" room in y check
            if (!((room.x + room.width + 1< check.x ) || (room.x - 1> check.x + check.width ) || (room.y + room.height - 1 < check.y) || (room.y + 1 > check.y + check.height)))
            {
                return true;
            }
        }
        return false;
    }


    /*
    void addCorridors()
    {
        bool thick = false;
        
        for (int i = 0; i < rooms.Length; i++)
        {
            room roomA = rooms[i];
            room roomB = findClosestRoom(roomA);

            Vector2 pointA = new Vector2(Random.Range(roomA.x, roomA.x + roomA.width + 1), Random.Range(roomA.y, roomA.y + roomA.height + 1));
            Vector2 pointB = new Vector2(Random.Range(roomB.x, roomB.x + roomB.width + 1), Random.Range(roomB.y, roomB.y + roomB.height + 1));
            Vector2 pointC = pointB + new Vector2(1, 0);

            //thicker hallway
            if (Random.Range(0, 10) >= 5)
            {
                thick = true;
            }

            while ((pointB.x != pointA.x) || (pointB.y != pointA.y))
            {
                if (pointB.x != pointA.x)
                {
                    if (pointB.x > pointA.x)
                    {
                        pointB.x--;
                        if (thick)
                        {
                            pointC.x--;
                        }
                    }
                    else
                    {
                        pointB.x++;
                        if (thick)
                        {
                            pointC.x++;
                        }
                    }
                }
                else if (pointB.y != pointA.y)
                {
                    if (pointB.y > pointA.y)
                    {
                        pointB.y--;
                        if (thick)
                        {
                            pointC.y--;
                        }
                    }
                    else
                    {
                        pointB.y++;
                        if (thick)
                        {
                            pointC.y++;
                        }
                    }
                }

                map[(int)pointB.x, (int)pointB.y] = 1;
                if (thick)
                {
                    map[(int)pointC.x, (int)pointC.y] = 1;
                }
            }

        }
    }
    */
    
    //connect rooms with hallways
    void testConnect()
    {
        rooms[0].connected = true; //set first room's connected to true

        for (int i = 0; i < rooms.Length; i++) //for each room
        {
            int numConnects = Random.Range(1, 3); //# of connects (1 or 2)
            
            for (int j = 0; j < numConnects; j++) //for each neighbor
            {
                connectRooms( rooms[i], findNeighbors(rooms[i])[j] ); //connect 1 or 2 neighbors
            }

            //if still not connected
            if (!rooms[i].connected) //connect to closest room
            {
                connectRooms(rooms[i], findNeighbors(rooms[i])[numConnects]);
            }
            
        }
    }

    //the problem
    //pick 2 random points between 2 rooms, have one (b) move towards another (a)
    void connectRooms(room a, room b)
    {
        //2 random points in 2 different rooms, move them towards each other
        bool thick = false;

        Vector2 pointA = new Vector2(Random.Range(a.x, a.x + a.width + 1), Random.Range(a.y, a.y + a.height + 1));
        Vector2 pointB = new Vector2(Random.Range(b.x, b.x + b.width + 1), Random.Range(b.y, b.y + b.height + 1));
        Vector2 pointC = pointB + new Vector2(1, 0);
        //thicker hallway
        if (Random.Range(0, 10) >= 8)
        {
            thick = true;
        }

        while ((pointB.x != pointA.x) || (pointB.y != pointA.y))
        {
            if (pointB.x != pointA.x)
            {
                if (pointB.x > pointA.x)
                {
                    pointB.x--;
                    if (thick)
                    {
                        pointC.x--;
                    }
                }
                else
                {
                    pointB.x++;
                    if (thick)
                    {
                        pointC.x++;
                    }
                }

            }
            else if (pointB.y != pointA.y)
            {
                if (pointB.y > pointA.y)
                {
                    pointB.y--;
                    if (thick)
                    {
                        pointC.y--;
                    }
                }
                else
                {
                    pointB.y++;
                    if (thick)
                    {
                        pointC.y++;
                    }
                }
            }
            if (pointB.x != 0 && pointB.y != 0)
            {
                //map[(int)pointB.x, (int)pointB.y] = 1;
            }

            //add floor tiles for hallways if it isnt already a floor tile
            if (map[(int)pointB.x, (int)pointB.y] != 1)
            {
                map[(int)pointB.x, (int)pointB.y] = 1;
                GameObject tile = (GameObject)Instantiate(ground, new Vector3((int)pointB.x, (int)pointB.y, 5), Quaternion.identity);
                tile.transform.parent = transform;
                if (thick)
                {
                    if (map[(int)pointC.x, (int)pointC.y] != 1)
                    {
                        map[(int)pointC.x, (int)pointC.y] = 1;
                        GameObject tile2 = (GameObject)Instantiate(ground, new Vector3((int)pointC.x, (int)pointC.y, 5), Quaternion.identity);
                        tile2.transform.parent = transform;
                    }
                }

            }
        }
        
        if (a.connected || b.connected)
        {
            a.connected = true;
            b.connected = true;
        }
    }

    //move rooms together - bigger tiemz, more compact
	void squash(int tiemz) {
		for (int j = 0; j < tiemz; j++) {
			for (int i = 0; i < rooms.Length; i++) { //for each room
				room curRoom = rooms[i];
                Vector2 oldPos = new Vector2(curRoom.x, curRoom.y);
                int maxMoves = 5; //only move this amount maximum
                int moveX = 0;
                int moveY = 0;
				//if x causes collision, y doesnt move
				if (curRoom.x + curRoom.width/2 > center.x) {
					//move to left
                    while (moveX < maxMoves)
                    {
                        //move
                        curRoom.move(-1, 0);
                        moveX++;
                        //if collide, move back one
                        if (doesCollideY(curRoom, i) || curRoom.x + curRoom.width / 2 < center.x)
                        {
                            curRoom.move(1,0);
                            break;
                        }
                       
                    }
                    /*
					while (!doesCollide(curRoom, i) && curRoom.x + curRoom.width/2 > center.x) {
						//print ("room#: " + i + " moving left: " + curRoom.x);
						curRoom.x--;
					}*/
				}
				else if (curRoom.x + curRoom.width/2 < center.x) {
					//move to right
                    while (moveX<maxMoves)
                    {
                        //move
                        curRoom.move(1,0);
                        moveX++;
                        //if collide, move back one
                        if (doesCollideY(curRoom, i) || curRoom.x + curRoom.width / 2 > center.x)
                        {
                            curRoom.move(-1, 0);
                            break;
                        }
                    }
                    /*
					while (!doesCollide(curRoom, i) && curRoom.x + curRoom.width/2 < center.x) {
						curRoom.x++;
					}*/
				}
				
				//y
				if (curRoom.y + curRoom.height/2 > center.y) {
					//move down
                    while (moveY < maxMoves)
                    {
                        //move
                        curRoom.move(0, -1);
                        moveY++;
                        //if collide, move back one
                        if (doesCollide(curRoom, i) || curRoom.y + curRoom.height / 2 < center.y)
                        {
                            curRoom.move(0, 1);
                            break;
                        }

                    }
                    /*
					while (!doesCollideY(curRoom, i) && curRoom.y + curRoom.height/2 > center.y) {
						curRoom.y--;
					}*/
				}
				else if (curRoom.y + curRoom.height/2 < center.y) {
					//move up
                    while (moveY < maxMoves)
                    {
                        //move
                        curRoom.move(0, 1);
                        moveY++;
                        //if collide, move back one
                        if (doesCollide(curRoom, i) || curRoom.y + curRoom.height / 2 > center.y)
                        {
                            curRoom.move(0, -1);
                            break;
                        }
                    }
                    /*
					while (!doesCollideY(curRoom, i) && curRoom.y + curRoom.height/2 < center.y) {
						curRoom.y++;
					}*/
				}

                curRoom.center = new Vector2(curRoom.x + (curRoom.width) / 2, curRoom.y + (curRoom.height) / 2);
				rooms[i] = curRoom;
			}
		}
	}

    room findClosestRoom(room room)
    {
        Vector2 mid = new Vector2(room.x + room.width/2, room.y + room.height/2);
        //room[] neighbors;
        //ArrayList neighborz = new ArrayList(4);
        //neighborz.Add(new room());
        room closest = new room();
        int closest_dist = 1000;
        for (int i = 0; i < rooms.Length; i++)
        {
            room check = rooms[i];
            if (check.index == room.index)
            {
                continue;
            }
            Vector2 check_mid = new Vector2(check.x + check.width / 2, check.y + check.height / 2);
            int distance = (int) (Mathf.Abs(mid.x - check_mid.x) +  Mathf.Abs(mid.y - check_mid.y));
            if (distance < closest_dist && room.neighbor != i)
            {
                //set to neightbors
                room.neighbor = i;
                rooms[room.index].neighbor = i;
                rooms[i].neighbor = room.index;
                closest_dist = distance;
                closest = check;
            }
            //else if (distance < 2nd closest) set to 2nd closest
        }
        return closest;
    }


    List<room> findNeighbors(room room)
    {
        List<room> neighbors = new List<room>();

        Vector2 mid = new Vector2(room.x + room.width / 2, room.y + room.height / 2);

        int[] closest_dist = new int[4] {1000, 1000, 1000, 1000} ;

        //loops thru each room and puts in neighbor list if close enough
        for (int i = 0; i < rooms.Length; i++)
        {
            room check = rooms[i];
            if (check.index == room.index)
            {
                continue;
            }
            
            Vector2 check_mid = new Vector2(check.x + check.width / 2, check.y + check.height / 2);
            int distance = (int)(Mathf.Abs(mid.x - check_mid.x) + Mathf.Abs(mid.y - check_mid.y));

            if (distance < closest_dist[0])
            {
                neighbors.Insert(0, rooms[i]);
                closest_dist[0] = distance;
            }
            else if (distance < closest_dist[1])
            {
                neighbors.Insert(1, rooms[i]);
                closest_dist[1] = distance;
            }
            else if (distance < closest_dist[2])
            {
                neighbors.Insert(2, rooms[i]);
                closest_dist[2] = distance;
            }
            else if (distance < closest_dist[3])
            {
                neighbors.Insert(3, rooms[i]);
                closest_dist[3] = distance;
            }
        }


        return neighbors;
    }

    /*
    //returns arraylist size 4 of neighbors to room
    ArrayList findNeighborz(room room)
    {
        Vector2 mid = new Vector2(room.x + room.width / 2, room.y + room.height / 2);


        ArrayList neighbors = new ArrayList(4);
        int[] closest_dist = new int[4];
        int count = 0;
        int closest = 1000;
        //ArrayList neighborz = new ArrayList(4);
        //neighborz.Add(new room());
        
        for (int i = 0; i < rooms.Length; i++)
        {
            if (count >= 4)
            {
                break;
            }
            room check = rooms[i];
            if (check.index == room.index)
            {
                continue;
            }

            Vector2 check_mid = new Vector2(check.x + check.width / 2, check.y + check.height / 2);
            int distance = (int)(Mathf.Abs(mid.x - check_mid.x) + Mathf.Abs(mid.y - check_mid.y));

            if (distance < closest && check.neighbor != i ) //hard coded
            {
                
                rooms[room.index].neighbor = i;
                rooms[i].neighbor = room.index;
                neighbors.Add(check);
                count++;

                closest = distance;
                //closest = check;
            }

        }
        return neighbors;
    }
    */

    void createMap()
    {
        for (int i = 0; i < numRooms; i++) //for each room
        {
            room currentRoom = rooms[i];
            currentRoom.updateCenter();

            //create empty game object room to put tiles in
            GameObject room = new GameObject("room " + i);
            room.transform.position = currentRoom.center;
            room.transform.parent = transform;

            currentRoom.roomObject = room;

            for (int x = currentRoom.x; x < currentRoom.x + currentRoom.width; x++)
            {
                for (int y = currentRoom.y; y < currentRoom.y + currentRoom.height; y++)
                {
                    //add tiles to empty game object
                    if (map[x,y] != 1)
                    {
                        map[x,y] = 1;
                        GameObject tile = (GameObject)Instantiate(ground, new Vector3(x, y, 5), Quaternion.identity);
                        tile.transform.parent = room.transform;
                    }
                }
            }
        }

        //draw wall tiles around rooms
        //can instantiate here and add to room gameobject if want to
        for (int x = 1; x < mapSize; x++)
        {
            for (int y = 1; y < mapSize; y++)
            {
                if (map[x, y] == 1)
                {
                    for (int xx = x - 1; xx <= x + 1; xx++)
                    {
                        for (int yy = y - 1; yy <= y + 1; yy++)
                        {
                            if (map[xx, yy] == 0 && map[xx, yy] != 3)
                            {
                                map[xx, yy] = 2;

                            }
                        }
                    }
                }
            }
        }
    }

    void createTiles()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                //ground
                if (map[x, y] == 1)
                {
                    //doing it above, dont need to do it again
                    //GameObject tile = (GameObject)Instantiate(ground, new Vector3(x, y, 5), Quaternion.identity);
                    //tile.transform.parent = transform;
                }
                //wall
                else if (map[x, y] == 2)
                {
                    GameObject tile = (GameObject)Instantiate(wall, new Vector3(x, y, 5), Quaternion.identity);
                    tile.transform.parent = transform;
                }

                else if (map[x, y] == 3)
                {
                    //GameObject tile = (GameObject)Instantiate(pink, new Vector3(x, y, 1), Quaternion.identity);
                    //tile.transform.parent = transform;                    
                }
            }
        }
    }

    public Vector3 getSpawnPosition()
    {
        int roomNum = Random.Range(0, (int)numRooms );
        int x = Random.Range(rooms[roomNum].x + 1, (int) (rooms[roomNum].x + rooms[roomNum].width));
        int y = Random.Range(rooms[roomNum].y + 1, (int)(rooms[roomNum].y + rooms[roomNum].height));
        print("spawn position:" + x + ", " + y);
        return new Vector3(x, y, 0);
        
    }

    public Vector3 spawnPortal(Vector3 playerSpawn)
    {
        List<int> farAwayRooms = new List<int>();
        for (int i = 0; i < numRooms; i++)
        {
            if (Vector3.Distance(playerSpawn, rooms[i].center) > mapSize / 3)
            {
                farAwayRooms.Add(i);
            }
        }
        for (int i = 0; i < farAwayRooms.Count; i++)
        {
            print(farAwayRooms[i] + " ");
        }
        int index = Random.Range(0, farAwayRooms.Count - 1);
        int roomNum = farAwayRooms[index];
        int x = Random.Range(rooms[roomNum].x + 2, (int)(rooms[roomNum].x + rooms[roomNum].width - 1));
        int y = Random.Range(rooms[roomNum].y + 2, (int)(rooms[roomNum].y + rooms[roomNum].height - 1));
        print("portal spawn position:" + x + ", " + y);
        return new Vector3(x, y, 1);
    }
}

public struct room {
    public GameObject roomObject;
    public int index;
	public int width, height;
	public int x, y;
	public Vector2 center;
    public int neighbor;
    public bool connected;
    //room[] neighbors;
    //public List<room> neighbors;
    public void move(int mx, int my)
    {
        x += mx;
        y += my;
        updateCenter();
    }
    public void updateCenter()
    {
        center = new Vector2(x + width / 2, y + height / 2);
    }
}