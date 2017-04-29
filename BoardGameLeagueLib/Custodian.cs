using System;
using System.Collections.Generic;

namespace BoardGameLeagueLib
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
    }
}
