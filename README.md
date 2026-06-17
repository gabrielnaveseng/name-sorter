# name-sorter

A small command-line application that sorts a list of person names by last name,
then by given names. The project is organized with a Ports & Adapters (hexagonal)
structure to separate domain logic from IO.

Features
- Read an input file containing one name per line (given names followed by last name).
- Sort by last name, then by given names (supports up to 3 given names).
- Write sorted output to `sorted-names-list.txt` and print results to console.
- Always generate an error file `invalid-names-list.txt` containing any invalid
	name entries (or an empty file when none exist).

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

The application also writes `invalid-names-list.txt` to the current working
directory. That file contains any parsed names that were considered invalid
because they have missing components (e.g. no given names or last name) or
have more than the allowed number of given names. The file is created even if
there are no invalid names (it will be empty in that case).

Testing

Run the unit tests:

```bash
dotnet test
```

Run the integration test:

```bash
dotnet test --filter Category=Integration
```

Design notes
- Architecture: Ports & Adapters (Application, Domain, Infrastructure layers).
- `PersonName` is a domain record. Parsing and file IO live in adapters.
- `IOrderNamesService` defines the application use case; DI is used in `Program`.