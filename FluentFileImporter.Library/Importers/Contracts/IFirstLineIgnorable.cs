using System;
using System.Collections.Generic;
using System.Text;

namespace FluentFileImporter.Library.Importers.Contracts
{
    public interface IFirstLineIgnorable<out T>
    {
        /// <summary>
        /// The first row or line of file shall be ignored; e.g. a header.
        /// False by default.
        /// </summary>
        bool FirstLineIgnored { get; }

        /// <summary>
        /// Configure importer to consider or ignore first line.
        /// </summary>
        /// <param name="ignore">
        ///   True to ignore, default value if this function is invoked without arguments.
        ///   False if explicitly stated in argument, but also false if this function is never invoked.</param>
        /// <returns></returns>
        T IgnoringFirstLine(bool ignore);
    }
}
