/*
TShock, a server mod for Terraria
Copyright (C) 2022 Janet Blackquill

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

using System.Text;

namespace TShockPluginManager
{
	static class CLIHelpers
	{
		static public bool YesNo()
		{
			System.Console.Write("[y/n] ");
			bool accept;
			bool confirm = false;
			do
			{
				ConsoleKey response = Console.ReadKey(true).Key;
				(accept, confirm) = response switch {
					ConsoleKey.Y => (true, true),
					ConsoleKey.N => (false, true),
					_ => (false, false)
				};
			} while (!confirm);
			if (accept)
				System.Console.WriteLine("yes");
			else
				System.Console.WriteLine("no");
			return accept;
		}
		public enum Answers {
			Yes,
			No,
			Explain
		}
		static public Answers YesNoExplain()
		{
			System.Console.Write("[y/n/e] ");
			Answers ans;
			bool confirm = false;
			do
			{
				ConsoleKey response = Console.ReadKey(true).Key;
				(ans, confirm) = response switch {
					ConsoleKey.Y => (Answers.Yes, true),
					ConsoleKey.N => (Answers.No, true),
					ConsoleKey.E => (Answers.Explain, true),
					_ => (Answers.Explain, false)
				};
			} while (!confirm);
			if (ans == Answers.Yes)
				System.Console.WriteLine("yes");
			else if (ans == Answers.No)
				System.Console.WriteLine("no");
			else
				System.Console.WriteLine("explain");
			return ans;
		}
		static private string[] ColorNames = Enum.GetNames(typeof(ConsoleColor));
		static public void Write(string text)
		{
			var initial = Console.ForegroundColor;

			var buffer = new StringBuilder();
			var chars = text.ToCharArray().ToList();
			while (chars.Count > 0)
			{
				var ch = chars.First();
				if (ch == '<')
				{
					var possibleColor = new string(chars.Skip(1).TakeWhile(c => c != '>').ToArray());
					Func<string, bool> predicate = x => string.Equals(x, possibleColor, StringComparison.CurrentCultureIgnoreCase);
					if (!ColorNames.Any(predicate))
						goto breakFromIf;
					var color = ColorNames.First(predicate);
					if (buffer.Length > 0)
					{
						Console.Write(buffer.ToString());
						buffer.Clear();
					}
					Console.ForegroundColor = Enum.Parse<ConsoleColor>(color);
					chars = chars.Skip(2 + possibleColor.Length).ToList();
					continue;
				}
			breakFromIf:
				buffer.Append(ch);
				chars.RemoveAt(0);
			}

			if (buffer.Length > 0)
				Console.Write(buffer.ToString());

			Console.ForegroundColor = initial;
		}
		static public void WriteLine(string text)
		{
			Write(text + "\n");
		}
	}
}
