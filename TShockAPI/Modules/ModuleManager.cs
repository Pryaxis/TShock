/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TShockAPI.Modules
{
	public class ModuleManager : IDisposable
	{
		private List<Module> _modules = new();

		/// <summary>
		/// Discovers <see cref="Module"/> derived classes from across the assembly
		/// </summary>
		/// <returns>Type definitions of the modules that can be created</returns>
		IEnumerable<Type> CollectModules() => Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(t => typeof(Module).IsAssignableFrom(t) && !t.IsAbstract)
		;

		/// <summary>
		/// Initialises <see cref="Module"/> derived classes defined across the assembly
		/// </summary>
		/// <param name="parameters">Additional constructor arguments allowed for modules</param>
		public void Initialise(object[] parameters)
		{
			foreach (var moduleType in CollectModules())
				InitialiseModule(moduleType, parameters);
		}

		/// <summary>
		/// Initialises a module by its type definition
		/// </summary>
		/// <param name="moduleType">The type of the module</param>
		/// <param name="parameters">Additional constructor arguments allowed for modules</param>
		public void InitialiseModule(Type moduleType, object[] parameters)
		{
			if (!typeof(Module).IsAssignableFrom(moduleType))
				throw new NotSupportedException($"Cannot load module {moduleType.FullName} as it does not derive from {typeof(Module).FullName}");

			var args = new List<object>();
			ConstructorInfo constructor = null;

			foreach (var ctor in moduleType.GetConstructors())
			{
				args.Clear();
				var ctorParams = ctor.GetParameters();

				foreach (var prm in ctorParams)
				{
					var matching_objects = parameters.Where(p => prm.ParameterType.IsAssignableFrom(p.GetType()));
					if (matching_objects.Count() == 1)
						args.Add(matching_objects.Single());
					else
					{
						// skip this ctor since we cannot find a suitable parameter for it.
						break;
					}
				}

				if (args.Count() == ctorParams.Length)
					constructor = ctor;
			}

			if (constructor is not null)
			{
				var module = (Module)constructor.Invoke(args.ToArray());
				_modules.Add(module);
				module.Initialise();
			}
		}

		/// <summary>
		/// Disposes of the module and the manager instance
		/// </summary>
		public void Dispose()
		{
			foreach (var module in _modules)
				module.Dispose();
			_modules.Clear();
		}
	}
}

