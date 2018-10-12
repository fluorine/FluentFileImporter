using System;
using System.Collections.Generic;

namespace FluentFileImporter.Importers.TextFile
{
    public interface ITextFileImporter
    {
        /// <summary>
        /// Columns defined in terms of substring, position at line and column length.
        /// </summary>
        IEnumerable<Tuple<int, int?>> DefinedColumns { get; }

        /// <summary>
        /// The first row or line of file shall be ignored. It may be a header.
        /// </summary>
        bool FirstLineIgnored { get; }
        
        /// <summary>
        /// Define a column in terms of substring, position at line and column length.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITextFileImporter WithColumn(int index, int? length = null);

        /// <summary>
        /// Configure importer to consider or ignore first line.
        /// </summary>
        /// <param name="ignore">True to ignore. False otherwise.</param>
        /// <returns></returns>
        ITextFileImporter IgnoreFirstLine(bool ignore);

        /// <summary>
        /// Method to assign method to adapt column values to an instance of <typeparamref name="E"/>.
        /// </summary>
        /// <typeparam name="E">Entity to be filled.</typeparam>
        /// <param name="entityAdapter">Method to adapt column values, given an empty instance of
        /// <typeparamref name="E"/> and a list of column values in order,
        /// defined with <see cref="WithColumn(int, int?)"/>. </param>
        /// <returns></returns>
        IFileImporter<E> AdaptTo<E>(Action<E, IList<string>> entityAdapter) where E : new();
    }
}