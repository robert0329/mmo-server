﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaelo_Server
{
    public class EntityManager
    {
        //Stores a list of each entity active in the game right now, sorted by their ID's
        public static Dictionary<string, ServerEntity> ActiveEntities = new Dictionary<string, ServerEntity>();
        public static Dictionary<string, ServerEntity> SecretNameEntities = new Dictionary<string, ServerEntity>();
        
        //Stores a new entity into the dictionary
        public static void AddNewEntity(ServerEntity NewEntity)
        {
            NewEntity.previousUpdatePosition = NewEntity.entity.Position;
            NewEntity.ID = EntityIDGenerator.GetNextID();
            ActiveEntities.Add(NewEntity.ID, NewEntity);
        }

        //Stores a new entity into the dictionary, with the special name to find it
        public static void AddNewEntity(ServerEntity NewEntity, string SecretName)
        {
            NewEntity.previousUpdatePosition = NewEntity.entity.Position;
            NewEntity.ID = EntityIDGenerator.GetNextID();
            ActiveEntities.Add(NewEntity.ID, NewEntity);
            SecretNameEntities.Add(SecretName, NewEntity);
        }

        //Return an entity from its secret name
        public static ServerEntity GetServerEntity(string SecretName)
        {
            return SecretNameEntities[SecretName];
        }

        //Returns all entities in a list
        public static List<ServerEntity> GetEntityList()
        {
            List<ServerEntity> EntityList = new List<ServerEntity>();
            foreach(var Entity in ActiveEntities)
                EntityList.Add(Entity.Value);
            return EntityList;
        }

        //Updates all entities on their current behaviour trees
        public static void UpdateEntities(float DeltaTime)
        {

        }

        //Returns a list of all entities who have moved to a new position since their last update
        public static List<ServerEntity> GetOutOfDateEntities()
        {
            //Make a list of entities who have moved
            List<ServerEntity> EntityList = new List<ServerEntity>();
            //We will need to check up on every entity that is active right now
            foreach(var Entity in ActiveEntities)
            {
                //If they havnt moved we wont add them into the update list
                if (Entity.Value.entity.Position == Entity.Value.previousUpdatePosition)
                    continue;
                //All the others are at new locations so must be added to the list
                EntityList.Add(Entity.Value);
                //Whenever entities are added into the update list, they need to update their previous position value
                Entity.Value.previousUpdatePosition = Entity.Value.entity.Position;
            }
            //Return the list of clients who need updating
            return EntityList;
        }
    }
}
