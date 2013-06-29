/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI {
  public static class PaginationTools {
    public delegate Tuple<string, Color> LineFormatterDelegate(object lineData, int lineIndex, int pageNumber);

    #region [Nested: Settings Class]
    public class Settings {
      public bool IncludeHeader { get; set; }

      private string headerFormat;
      public string HeaderFormat
      {
        get { return this.headerFormat; }
        set
        {
          if (value == null)
            throw new ArgumentNullException();

          this.headerFormat = value;
        }
      }

      public Color HeaderTextColor { get; set; }
      public bool IncludeFooter { get; set; }

      private string footerFormat;
      public string FooterFormat
      {
        get { return this.footerFormat; }
        set
        {
          if (value == null)
            throw new ArgumentNullException();

          this.footerFormat = value;
        }
      }

      public Color FooterTextColor { get; set; }
      public string NothingToDisplayString { get; set; }
      public LineFormatterDelegate LineFormatter { get; set; }
      public Color LineTextColor { get; set; }

      private int maxLinesPerPage;

      public int MaxLinesPerPage
      {
        get { return this.maxLinesPerPage; }
        set
        {
          if (value <= 0)
            throw new ArgumentException("The value has to be greater than zero.");

          this.maxLinesPerPage = value;
        }
      }

      private int pageLimit;

      public int PageLimit
      {
        get { return this.pageLimit; }
        set
        {
          if (value < 0)
            throw new ArgumentException("The value has to be greater than or equal to zero.");

          this.pageLimit = value;
        }
      }


      public Settings()
      {
        this.IncludeHeader = true;
        this.headerFormat = "Page {0} of {1}";
        this.HeaderTextColor = Color.Green;
        this.IncludeFooter = true;
        this.footerFormat = "Type /<command> {0} for more.";
        this.FooterTextColor = Color.Yellow;
        this.NothingToDisplayString = null;
        this.LineFormatter = null;
        this.LineTextColor = Color.White;
        this.maxLinesPerPage = 4;
        this.pageLimit = 0;
      }
    }
    #endregion

    public static void SendPage(
      TSPlayer player, int pageNumber, IEnumerable dataToPaginate, int dataToPaginateCount, Settings settings = null)
    {
      if (settings == null)
        settings = new Settings();

      if (dataToPaginateCount == 0)
      {
        if (settings.NothingToDisplayString != null)
          player.SendMessage(settings.NothingToDisplayString, settings.HeaderTextColor);

        return;
      }

      int pageCount = ((dataToPaginateCount - 1) / settings.MaxLinesPerPage) + 1;
      if (settings.PageLimit > 0 && pageCount > settings.PageLimit)
        pageCount = settings.PageLimit;
      if (pageNumber > pageCount)
        pageNumber = pageCount;

      if (settings.IncludeHeader)
        player.SendMessage(string.Format(settings.HeaderFormat, pageNumber, pageCount), settings.HeaderTextColor);

      int listOffset = (pageNumber - 1) * settings.MaxLinesPerPage;
      int offsetCounter = 0;
      int lineCounter = 0;
      foreach (object lineData in dataToPaginate)
      {
        if (lineData == null)
          continue;
        if (offsetCounter++ < listOffset)
          continue;
        if (lineCounter++ == settings.MaxLinesPerPage)
          break;

        string lineMessage;
        Color lineColor = settings.LineTextColor;
        if (lineData is Tuple<string, Color>)
        {
          var lineFormat = (Tuple<string, Color>)lineData;
          lineMessage = lineFormat.Item1;
          lineColor = lineFormat.Item2;
        }
        else if (settings.LineFormatter != null)
        {
          try
          {
            Tuple<string, Color> lineFormat = settings.LineFormatter(lineData, offsetCounter, pageNumber);
            if (lineFormat == null)
              continue;

            lineMessage = lineFormat.Item1;
            lineColor = lineFormat.Item2;
          }
          catch (Exception ex)
          {
            throw new InvalidOperationException(
              "The method referenced by LineFormatter has thrown an exception. See inner exception for details.", ex);
          }
        }
        else
        {
          lineMessage = lineData.ToString();
        }

        if (lineMessage != null)
          player.SendMessage(lineMessage, lineColor);
      }

      if (lineCounter == 0)
      {
        if (settings.NothingToDisplayString != null)
          player.SendMessage(settings.NothingToDisplayString, settings.HeaderTextColor);
      }
      else if (settings.IncludeFooter && pageNumber + 1 <= pageCount)
      {
        player.SendMessage(string.Format(settings.FooterFormat, pageNumber + 1, pageNumber, pageCount), settings.FooterTextColor);
      }
    }

    public static void SendPage(TSPlayer player, int pageNumber, IList dataToPaginate, Settings settings = null)
    {
      PaginationTools.SendPage(player, pageNumber, dataToPaginate, dataToPaginate.Count, settings);
    }

    public static List<string> BuildLinesFromTerms(
      IEnumerable terms, Func<object, string> termFormatter = null, string separator = ", ", int maxCharsPerLine = 80)
    {
      List<string> lines = new List<string>();
      StringBuilder lineBuilder = new StringBuilder();
      foreach (object term in terms)
      {
        if (term == null && termFormatter == null)
          continue;

        string termString;
        if (termFormatter != null)
        {
          try {
            termString = termFormatter(term);

            if (termString == null)
              continue;
          } catch (Exception ex)
          {
            throw new ArgumentException(
              "The method represented by termFormatter has thrown an exception. See inner exception for details.", ex);
          }
        }
        else
        {
          termString = term.ToString();
        }

        bool goesOnNextLine = (lineBuilder.Length + termString.Length > maxCharsPerLine);
        if (!goesOnNextLine)
        {
          if (lineBuilder.Length > 0) {
            lineBuilder.Append(separator);
          }
          lineBuilder.Append(termString);
        }
        else
        {
          // A separator should always be at the end of a line as we know it is followed by another line.
          lineBuilder.Append(separator);
          lines.Add(lineBuilder.ToString());
          lineBuilder.Clear();

          lineBuilder.Append(termString);
        }
      }
      if (lineBuilder.Length > 0)
        lines.Add(lineBuilder.ToString());

      return lines;
    }

    public static bool TryParsePageNumber(
      List<string> commandParameters, int expectedParamterIndex, TSPlayer errorMessageReceiver, out int pageNumber)
    {
      pageNumber = 1;
      if (commandParameters.Count <= expectedParamterIndex)
        return true;

      string pageNumberRaw = commandParameters[expectedParamterIndex];
      if (!int.TryParse(pageNumberRaw, out pageNumber) || pageNumber < 1)
      {
        if (errorMessageReceiver != null)
          errorMessageReceiver.SendErrorMessage(string.Format("\"{0}\" is not a valid page number.", pageNumberRaw));

        pageNumber = 1;
        return false;
      }

      return true;
    }
  }
}