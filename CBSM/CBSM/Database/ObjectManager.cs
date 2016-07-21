using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database
{
    class ObjectManager
    {
        private List<KnownObject> knownObjects;

        private static ObjectManager instance;
        private static ObjectManager INSTANCE
        {
            get
            {
                if (instance == null)
                    instance = new ObjectManager();

                return instance;
            }
        }
        
        private ObjectManager()
        {
            this.knownObjects = new List<KnownObject>();
        }

        private KnownObject FindObject(int id, Type table)
        {
            for(int i = 0; i<knownObjects.Count; i++)
            {
                if (knownObjects[i].CreationTime.AddMinutes(1) < DateTime.Now)
                {
                    // Object is outdated
                    knownObjects.RemoveAt(i);
                    i--;
                    continue;
                }
                if (knownObjects[i].Id == id && knownObjects[i].Table == table)
                {
                    return knownObjects[i];
                }
            }

            return null;
        }

        public static void AddObject(int id, Type table, object instance)
        {
            KnownObject known = INSTANCE.FindObject(id, table);
            if (known != null)
            {
                known.ResetTime();
                return;
            }

            INSTANCE.knownObjects.Add(new KnownObject(id, table, instance));
        }

        public static bool DoesObjectExsist(int id, Type table)
        {
            return INSTANCE.FindObject(id, table) != null;
        }
           
        public static object GetObject(int id, Type table)
        {
            return INSTANCE.FindObject(id, table).Object;
        }
    }
}
