using System;
using System.Collections.Generic;
using System.Text;

namespace UIExpansionKit.API.Layout;

public class TableLayout : IHtmlLayout<TableLayout.Param>
{
    public readonly int Columns;
    private readonly List<(string Control, Param? Param)> myControls = new();

    public TableLayout(int columns)
    {
        if (columns <= 0)
            throw new ArgumentOutOfRangeException(nameof(columns), columns, "Columns bust be greater than 0");
        
        Columns = columns;
    }

    public void Clear()
    {
        myControls.Clear();
    }

    public void AddControl(string controlHtmlString, Param? parameter)
    {
        myControls.Add((controlHtmlString, parameter));
    }

    public string GetHtml()
    {
        var builder = new StringBuilder();
        builder.Append("<table>");
        builder.Append("<tr>");
        
        var columnCounter = 0;
        foreach (var pair in myControls)
        {
            var param = pair.Param;
            builder.Append("<td");
            if (param != null && param.Value.ColumnSpan > 1)
            {
                builder.Append(" colspan=\"");
                builder.Append(param.Value.ColumnSpan);
                builder.Append("\"");
            }
            if (param != null && param.Value.RowSpan > 1)
            {
                builder.Append(" rowspan=\"");
                builder.Append(param.Value.RowSpan);
                builder.Append("\"");
            }

            builder.Append(">");
            builder.Append(pair.Control);
            builder.Append("</td>");
            columnCounter += param?.RowSpan ?? 1;
            if (columnCounter >= Columns)
            {
                builder.Append("</tr><tr>");
                columnCounter = 0;
            }
        }

        builder.Append("</tr>");
        builder.Append("</table>");
        return builder.ToString();
    }


    public struct Param
    {
        public int RowSpan;
        public int ColumnSpan;
    }
}