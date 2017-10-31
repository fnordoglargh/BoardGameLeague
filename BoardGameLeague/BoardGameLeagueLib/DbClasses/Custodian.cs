using System;
using System.Collections.Generic;

namespace BoardGameLeagueLib.DbClasses
{
    public sealed class Custodian
    {
        private HashSet<Guid> m_Ids;
        private static readonly Custodian m_Instance = new Custodian();

        private Custodian()
        {
            m_Ids = new HashSet<Guid>();
        }

        public static Custodian Instance
        {
            get
            {
                return m_Instance;
            }
        }

        /// <summary>
        /// Generates a Guid which is guaranteed to be unique among all used IDs.
        /// </summary>
        /// <returns>A new Guid.</returns>
        public Guid GenerateUuid()
        {
            Guid v_IdTest;

            do
            {
                v_IdTest = System.Guid.NewGuid();
            } while (m_Ids.Contains(v_IdTest));

            m_Ids.Add(v_IdTest);

            return v_IdTest;
        }

        /// <summary>
        /// Tests if an ID is already used.
        /// </summary>
        /// <param name="a_Id">The Guid to test.</param>
        /// <returns>True if the ID has been added or false if it was not.</returns>
        public bool AddAndTestId(Guid a_Id)
        {
            bool v_IsAdded = false;

            if (!m_Ids.Contains(a_Id))
            {
                m_Ids.Add(a_Id);
                v_IsAdded = true;
            }

            return v_IsAdded;
        }

        /// <summary>
        /// Used to reinitialize the used IDs e.g. if another database is loaded.
        /// </summary>
        public void Reset()
        {
            m_Ids.Clear();
        }
    }
}
