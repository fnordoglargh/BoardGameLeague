using System;

public sealed class CellIndexer
{
    private static readonly Lazy<CellIndexer> lazy = new Lazy<CellIndexer>(() => new CellIndexer());
    public static CellIndexer Instance { get { return lazy.Value; } }

    private int m_ActualIndex = 0;
    private int m_MaximumElements = int.MaxValue;
    const int c_DefaultElementsPerRow = 10;
    private int m_ElementsPerRow = c_DefaultElementsPerRow;

    public int ActualIndex
    {
        get
        {
            return m_ActualIndex;
        }
        private set
        {
            if (value >= m_MaximumElements)
            {
                m_ActualIndex = 0;
            }
            else
            {
                m_ActualIndex = value;
            }
        }
    }

    public int RowIndex
    {
        get
        {
            return (ActualIndex++) % m_ElementsPerRow;
        }
    }

    public int ElementsPerRow { get { return m_ElementsPerRow; } }

    public void Reset(int a_MaximumElements, int a_ElementsPerRow)
    {
        m_ActualIndex = 0;

        if (a_MaximumElements < 2)
        {
            m_MaximumElements = 1;
        }
        else if (a_MaximumElements > 1)
        {
            m_MaximumElements = a_MaximumElements;
        }

        if (a_ElementsPerRow < 1)
        {
            m_ElementsPerRow = 1;
        }
        else if (a_ElementsPerRow > 0)
        {
            m_ElementsPerRow = a_ElementsPerRow;
        }
    }
}