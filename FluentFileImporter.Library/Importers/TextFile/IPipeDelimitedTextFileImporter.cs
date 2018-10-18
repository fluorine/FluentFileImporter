using System;
using System.Collections.Generic;
using FluentFileImporter.Library.Importers.Contracts;

namespace FluentFileImporter.Importers.TextFile
{
    /// <summary>
    /// Defines an importer for files with pipe-delimited (|) columns.
    ///   IMPORTANT: The importer assumes the pipe could be scaped with the sequence "\|"
    /// </summary>
    public interface IPipeDelimitedTextFileImporter : IFirstLineIgnorable<IPipeDelimitedTextFileImporter>
    {
        /// <summary>
        /// Consider multiline values. See <see cref="ConsideringMultilineValues"/> documentation for details.
        /// </summary>
        bool MultilineValuesConsidered { get; }

        /// <summary>
        /// Multiline values are considered if the setting <see cref="MultilineValuesConsidered"/> is <value>true</value>.
        /// The setting can be configured using this function.
        /// If there are not enough column values for the defined.
        /// </summary>
        /// <param name="considerMultilineValues"></param>
        IPipeDelimitedTextFileImporter ConsideringMultilineValues(bool considerMultilineValues = true);

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
        /// <typeparamref name="E"/> and a list of column values as key values,
        /// defined with <see cref="HasColumn(int, string)"/>. </param>
        /// <returns></returns>
        IFileImporter<E> AdaptTo<E>(Action<E, IDictionary<string, string>> entityAdapter) where E : new();
    }
}