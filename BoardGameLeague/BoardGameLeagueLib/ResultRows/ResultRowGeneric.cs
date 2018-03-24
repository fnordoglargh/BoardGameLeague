using BoardGameLeagueLib.DbClasses;
using System.Collections.ObjectModel;

namespace BoardGameLeagueLib.ResultRows
{
    // Adapted from http://paulstovell.com/blog/dynamic-datagrid
    public class ResultRowGeneric
    {
        private readonly ObservableCollection<GenericProperty> m_Properties = new ObservableCollection<GenericProperty>();

        public ResultRowGeneric(params GenericProperty[] a_Properties)
        {
            foreach (var i_Property in a_Properties)
            {
                Properties.Add(i_Property);
            }
        }

        public ObservableCollection<GenericProperty> Properties
        {
            get { return m_Properties; }
        }
    }
}
