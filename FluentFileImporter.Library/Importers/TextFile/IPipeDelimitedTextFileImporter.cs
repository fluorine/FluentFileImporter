using System;
using System.Collections.Generic;
using FluentFileImporter.Library.Importers.Contracts;

namespace FluentFileImporter.Importers.TextFile
{
    public interface IPipeDelimitedTextFileImporter : IFirstLineIgnorable<IPipeDelimitedTextFileImporter>
    {
        /// <summary>
        /// Columns defined in terms of substring, position at line and column length.
        /// </summary>
        IDictionary<int, string> DefinedColumns { get; }

        /// <summary>
        /// Define a column in terms of substring, position at line and column length.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        IPipeDelimitedTextFileImporter HasColumn(int index, string named);

        /// <summary>
        /// Method to assign method to adapt column values to an instance of <typeparamref name="E"/>.
        /// </summary>
        /// <typeparam name="E">Entity to be filled.</typeparam>
        /// <param name="entityAdapter">Method to adapt column values, given an empty instance of
        /// <typeparamref name="E"/> and a list of column values in order,
        /// defined with <see cref="HasColumn(int, int?)"/>. </param>
        /// <returns></returns>
        IFileImporter<E> AdaptTo<E>(Action<E, IDictionary<int, string>> entityAdapter) where E : new();
    }
}