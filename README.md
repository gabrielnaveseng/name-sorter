# name-sorter

A small command-line application that sorts a list of person names by last name,
then by given names. The project is organized with a Ports & Adapters (hexagonal)
structure to separate domain logic from IO.

Features
- Read an input file containing one name per line (given names followed by last name).
- Sort by last name, then by given names (supports up to 3 given names).
- Write sorted output to `sorted-names-list.txt` and print results to console.

Quick start

Prerequisites: .NET 10 SDK

Run the app (from repository root):

```bash
dotnet run --project NameSorter -- <path-to-input-file>
```

Example:

```bash
dotnet run --project NameSorter -- NameSorter/unsorted-names-list.txt
```

This writes `sorted-names-list.txt` in the current working directory and prints
the sorted names to stdout.

Testing

Run the unit tests:

```bash
dotnet test
```

Design notes
- Architecture: Ports & Adapters (Application, Domain, Infrastructure layers).
- `PersonName` is a domain record. Parsing and file IO live in adapters.
- `IOrderNamesService` defines the application use case; DI is used in `Program`.