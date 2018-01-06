using System;
using System.ComponentModel;

namespace BoardGameLeagueLib.DbClasses
{
    public abstract class DbObject: INotifyPropertyChanged
    {
        private Guid m_Id;

        public Guid Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                if (Custodian.Instance.AddAndTestId(value))
                {
                    m_Id = value;
                }
                else
                {
                    m_Id = Custodian.Instance.GenerateUuid();
                }
            }
        }

        public DbObject()
        {
            Id = Custodian.Instance.GenerateUuid();
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String a_PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(a_PropertyName));
        }

        #endregion

        //public DbObject(Guid a_Id)
        //{
        //    Custodian v_Custodian = Custodian.Instance;
        //    bool v_IsAdded = v_Custodian.AddAndTestId(a_Id);

        //    if (v_IsAdded)
        //    {
        //        Id = a_Id;
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Supplied Id is invalid because it is allrady present in database. Id was: " + a_Id);
        //    }
        //}
    }
}
