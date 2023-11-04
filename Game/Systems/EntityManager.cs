using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems
{
    public class EntityManager
    {
        private int nextEntityId = 0;
        private Dictionary<int, List<object>> componentsByEntity = new Dictionary<int, List<object>>();
        private Dictionary<Type, Dictionary<int, object>> componentTypeByEntity = new Dictionary<Type, Dictionary<int, object>>();

        public int CreateEntity()
        {
            int entityId = nextEntityId++;
            componentsByEntity.Add(entityId, new List<object>());
            return entityId;
        }

        public void AddComponent<T>(int entityId, T component)
        {
            componentsByEntity[entityId].Add(component);

            // Add component to type dictionary for fast access
            var type = typeof(T);
            if (!componentTypeByEntity.ContainsKey(type))
            {
                componentTypeByEntity[type] = new Dictionary<int, object>();
            }
            componentTypeByEntity[type][entityId] = component;
        }

        public void ClearAllEntities()
        {
            // Clears all components associated with each entity
            componentsByEntity.Clear();

            // Clears all entities from each component type
            foreach (var pair in componentTypeByEntity)
            {
                pair.Value.Clear();
            }

            // Reset the entity ID counter if necessary
            nextEntityId = 0;
        }

        public bool HasComponent<T>(int entityId)
        {
            var type = typeof(T);
            if (!componentTypeByEntity.ContainsKey(type))
            {
                return false;
            }
            return componentTypeByEntity[type].ContainsKey(entityId);
        }

        public T GetComponent<T>(int entityId)
        {
            var type = typeof(T);
            if (!componentTypeByEntity.ContainsKey(type))
            {
                throw new Exception($"No component of type {type} found for entity {entityId}");
            }

            if (!componentTypeByEntity[type].ContainsKey(entityId))
            {
                throw new Exception($"Entity {entityId} does not have component of type {type}");
            }

            return (T)componentTypeByEntity[type][entityId];
        }

        public List<int> GetAllEntitiesWithComponent<T>() where T : struct
        {
            var type = typeof(T);
            if (!componentTypeByEntity.ContainsKey(type))
            {
                return new List<int>(); // Return an empty list if the component type doesn't exist
            }

            return componentTypeByEntity[type].Keys.ToList();
        }

        public IEnumerable<int> GetAllEntities()
        {
            return componentsByEntity.Keys;
        }

        public void UpdateComponent<T>(int entityId, T component)
        {
            var type = typeof(T);
            if (!componentTypeByEntity.ContainsKey(type) || !componentTypeByEntity[type].ContainsKey(entityId))
            {
                throw new Exception($"Entity {entityId} does not have component of type {type} to update.");
            }

            // Update the component in the type dictionary
            componentTypeByEntity[type][entityId] = component;

            // Find and update the component in the list of components for the entity
            var componentsList = componentsByEntity[entityId];
            for (int i = 0; i < componentsList.Count; i++)
            {
                if (componentsList[i].GetType() == type)
                {
                    componentsList[i] = component;
                    return; // Exit the method once the component is updated
                }
            }
        }

        public void RemoveComponent<T>(int entityId)
        {
            var type = typeof(T);
            if (!componentTypeByEntity.ContainsKey(type) || !componentTypeByEntity[type].ContainsKey(entityId))
            {
                throw new Exception($"Entity {entityId} does not have component of type {type} to remove.");
            }

            // Remove the component from the type dictionary
            componentTypeByEntity[type].Remove(entityId);

            // Find and remove the component from the list of components for the entity
            var componentsList = componentsByEntity[entityId];
            for (int i = 0; i < componentsList.Count; i++)
            {
                if (componentsList[i].GetType() == type)
                {
                    componentsList.RemoveAt(i);
                    return; // Exit the method once the component is removed
                }
            }
        }

        // Other methods to remove components and entities...
    }
}
