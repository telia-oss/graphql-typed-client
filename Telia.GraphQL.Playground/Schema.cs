namespace Telia.GraphQL.Playground
{
    using System;
    using System.Collections.Generic;
    using Telia.GraphQL.Client.Attributes;

    [GraphQLType("Film")]
    public class Film : Node
    {
        [GraphQLField("title", "String")]
        public virtual String Title { get; set; }

        [GraphQLField("episodeID", "Int")]
        public virtual Int32? EpisodeId { get; set; }

        [GraphQLField("openingCrawl", "String")]
        public virtual String OpeningCrawl { get; set; }

        [GraphQLField("director", "String")]
        public virtual String Director { get; set; }

        [GraphQLField("producers", "[String]")]
        public virtual IEnumerable<String> Producers { get; set; }

        [GraphQLField("releaseDate", "String")]
        public virtual String ReleaseDate { get; set; }

        [GraphQLField("speciesConnection", "FilmSpeciesConnection")]
        public virtual FilmSpeciesConnection SpeciesConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("speciesConnection", "FilmSpeciesConnection")]
        public virtual FilmSpeciesConnection SpeciesConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("speciesConnection", "FilmSpeciesConnection")]
        public virtual FilmSpeciesConnection SpeciesConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("speciesConnection", "FilmSpeciesConnection")]
        public virtual FilmSpeciesConnection SpeciesConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("speciesConnection", "FilmSpeciesConnection")]
        public virtual FilmSpeciesConnection SpeciesConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "FilmStarshipsConnection")]
        public virtual FilmStarshipsConnection StarshipConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "FilmStarshipsConnection")]
        public virtual FilmStarshipsConnection StarshipConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "FilmStarshipsConnection")]
        public virtual FilmStarshipsConnection StarshipConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "FilmStarshipsConnection")]
        public virtual FilmStarshipsConnection StarshipConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "FilmStarshipsConnection")]
        public virtual FilmStarshipsConnection StarshipConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "FilmVehiclesConnection")]
        public virtual FilmVehiclesConnection VehicleConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "FilmVehiclesConnection")]
        public virtual FilmVehiclesConnection VehicleConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "FilmVehiclesConnection")]
        public virtual FilmVehiclesConnection VehicleConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "FilmVehiclesConnection")]
        public virtual FilmVehiclesConnection VehicleConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "FilmVehiclesConnection")]
        public virtual FilmVehiclesConnection VehicleConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("characterConnection", "FilmCharactersConnection")]
        public virtual FilmCharactersConnection CharacterConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("characterConnection", "FilmCharactersConnection")]
        public virtual FilmCharactersConnection CharacterConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("characterConnection", "FilmCharactersConnection")]
        public virtual FilmCharactersConnection CharacterConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("characterConnection", "FilmCharactersConnection")]
        public virtual FilmCharactersConnection CharacterConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("characterConnection", "FilmCharactersConnection")]
        public virtual FilmCharactersConnection CharacterConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("planetConnection", "FilmPlanetsConnection")]
        public virtual FilmPlanetsConnection PlanetConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("planetConnection", "FilmPlanetsConnection")]
        public virtual FilmPlanetsConnection PlanetConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("planetConnection", "FilmPlanetsConnection")]
        public virtual FilmPlanetsConnection PlanetConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("planetConnection", "FilmPlanetsConnection")]
        public virtual FilmPlanetsConnection PlanetConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("planetConnection", "FilmPlanetsConnection")]
        public virtual FilmPlanetsConnection PlanetConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("created", "String")]
        public virtual String Created { get; set; }

        [GraphQLField("edited", "String")]
        public virtual String Edited { get; set; }

        [GraphQLField("id", "ID!")]
        public virtual String Id { get; set; }
    }

    [GraphQLType("FilmCharactersConnection")]
    public class FilmCharactersConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[FilmCharactersEdge]")]
        public virtual IEnumerable<FilmCharactersEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("characters", "[Person]")]
        public virtual IEnumerable<Person> Characters { get; set; }
    }

    [GraphQLType("FilmCharactersEdge")]
    public class FilmCharactersEdge
    {
        [GraphQLField("node", "Person")]
        public virtual Person Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("FilmPlanetsConnection")]
    public class FilmPlanetsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[FilmPlanetsEdge]")]
        public virtual IEnumerable<FilmPlanetsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("planets", "[Planet]")]
        public virtual IEnumerable<Planet> Planets { get; set; }
    }

    [GraphQLType("FilmPlanetsEdge")]
    public class FilmPlanetsEdge
    {
        [GraphQLField("node", "Planet")]
        public virtual Planet Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("FilmsConnection")]
    public class FilmsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[FilmsEdge]")]
        public virtual IEnumerable<FilmsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("films", "[Film]")]
        public virtual IEnumerable<Film> Films { get; set; }
    }

    [GraphQLType("FilmsEdge")]
    public class FilmsEdge
    {
        [GraphQLField("node", "Film")]
        public virtual Film Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("FilmSpeciesConnection")]
    public class FilmSpeciesConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[FilmSpeciesEdge]")]
        public virtual IEnumerable<FilmSpeciesEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("species", "[Species]")]
        public virtual IEnumerable<Species> Species { get; set; }
    }

    [GraphQLType("FilmSpeciesEdge")]
    public class FilmSpeciesEdge
    {
        [GraphQLField("node", "Species")]
        public virtual Species Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("FilmStarshipsConnection")]
    public class FilmStarshipsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[FilmStarshipsEdge]")]
        public virtual IEnumerable<FilmStarshipsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("starships", "[Starship]")]
        public virtual IEnumerable<Starship> Starships { get; set; }
    }

    [GraphQLType("FilmStarshipsEdge")]
    public class FilmStarshipsEdge
    {
        [GraphQLField("node", "Starship")]
        public virtual Starship Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("FilmVehiclesConnection")]
    public class FilmVehiclesConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[FilmVehiclesEdge]")]
        public virtual IEnumerable<FilmVehiclesEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("vehicles", "[Vehicle]")]
        public virtual IEnumerable<Vehicle> Vehicles { get; set; }
    }

    [GraphQLType("FilmVehiclesEdge")]
    public class FilmVehiclesEdge
    {
        [GraphQLField("node", "Vehicle")]
        public virtual Vehicle Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("Node")]
    public interface Node
    {
        [GraphQLField("id", "ID!")]
        String Id { get; set; }
    }

    [GraphQLType("PageInfo")]
    public class PageInfo
    {
        [GraphQLField("hasNextPage", "Boolean!")]
        public virtual Boolean HasNextPage { get; set; }

        [GraphQLField("hasPreviousPage", "Boolean!")]
        public virtual Boolean HasPreviousPage { get; set; }

        [GraphQLField("startCursor", "String")]
        public virtual String StartCursor { get; set; }

        [GraphQLField("endCursor", "String")]
        public virtual String EndCursor { get; set; }
    }

    [GraphQLType("PeopleConnection")]
    public class PeopleConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[PeopleEdge]")]
        public virtual IEnumerable<PeopleEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("people", "[Person]")]
        public virtual IEnumerable<Person> People { get; set; }
    }

    [GraphQLType("PeopleEdge")]
    public class PeopleEdge
    {
        [GraphQLField("node", "Person")]
        public virtual Person Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("Person")]
    public class Person : Node
    {
        [GraphQLField("name", "String")]
        public virtual String Name { get; set; }

        [GraphQLField("birthYear", "String")]
        public virtual String BirthYear { get; set; }

        [GraphQLField("eyeColor", "String")]
        public virtual String EyeColor { get; set; }

        [GraphQLField("gender", "String")]
        public virtual String Gender { get; set; }

        [GraphQLField("hairColor", "String")]
        public virtual String HairColor { get; set; }

        [GraphQLField("height", "Int")]
        public virtual Int32? Height { get; set; }

        [GraphQLField("mass", "Float")]
        public virtual Single? Mass { get; set; }

        [GraphQLField("skinColor", "String")]
        public virtual String SkinColor { get; set; }

        [GraphQLField("homeworld", "Planet")]
        public virtual Planet Homeworld { get; set; }

        [GraphQLField("filmConnection", "PersonFilmsConnection")]
        public virtual PersonFilmsConnection FilmConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PersonFilmsConnection")]
        public virtual PersonFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PersonFilmsConnection")]
        public virtual PersonFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PersonFilmsConnection")]
        public virtual PersonFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PersonFilmsConnection")]
        public virtual PersonFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("species", "Species")]
        public virtual Species SpeciesField { get; set; }

        [GraphQLField("starshipConnection", "PersonStarshipsConnection")]
        public virtual PersonStarshipsConnection StarshipConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "PersonStarshipsConnection")]
        public virtual PersonStarshipsConnection StarshipConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "PersonStarshipsConnection")]
        public virtual PersonStarshipsConnection StarshipConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "PersonStarshipsConnection")]
        public virtual PersonStarshipsConnection StarshipConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starshipConnection", "PersonStarshipsConnection")]
        public virtual PersonStarshipsConnection StarshipConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "PersonVehiclesConnection")]
        public virtual PersonVehiclesConnection VehicleConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "PersonVehiclesConnection")]
        public virtual PersonVehiclesConnection VehicleConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "PersonVehiclesConnection")]
        public virtual PersonVehiclesConnection VehicleConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "PersonVehiclesConnection")]
        public virtual PersonVehiclesConnection VehicleConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicleConnection", "PersonVehiclesConnection")]
        public virtual PersonVehiclesConnection VehicleConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("created", "String")]
        public virtual String Created { get; set; }

        [GraphQLField("edited", "String")]
        public virtual String Edited { get; set; }

        [GraphQLField("id", "ID!")]
        public virtual String Id { get; set; }
    }

    [GraphQLType("PersonFilmsConnection")]
    public class PersonFilmsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[PersonFilmsEdge]")]
        public virtual IEnumerable<PersonFilmsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("films", "[Film]")]
        public virtual IEnumerable<Film> Films { get; set; }
    }

    [GraphQLType("PersonFilmsEdge")]
    public class PersonFilmsEdge
    {
        [GraphQLField("node", "Film")]
        public virtual Film Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("PersonStarshipsConnection")]
    public class PersonStarshipsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[PersonStarshipsEdge]")]
        public virtual IEnumerable<PersonStarshipsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("starships", "[Starship]")]
        public virtual IEnumerable<Starship> Starships { get; set; }
    }

    [GraphQLType("PersonStarshipsEdge")]
    public class PersonStarshipsEdge
    {
        [GraphQLField("node", "Starship")]
        public virtual Starship Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("PersonVehiclesConnection")]
    public class PersonVehiclesConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[PersonVehiclesEdge]")]
        public virtual IEnumerable<PersonVehiclesEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("vehicles", "[Vehicle]")]
        public virtual IEnumerable<Vehicle> Vehicles { get; set; }
    }

    [GraphQLType("PersonVehiclesEdge")]
    public class PersonVehiclesEdge
    {
        [GraphQLField("node", "Vehicle")]
        public virtual Vehicle Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("Planet")]
    public class Planet : Node
    {
        [GraphQLField("name", "String")]
        public virtual String Name { get; set; }

        [GraphQLField("diameter", "Int")]
        public virtual Int32? Diameter { get; set; }

        [GraphQLField("rotationPeriod", "Int")]
        public virtual Int32? RotationPeriod { get; set; }

        [GraphQLField("orbitalPeriod", "Int")]
        public virtual Int32? OrbitalPeriod { get; set; }

        [GraphQLField("gravity", "String")]
        public virtual String Gravity { get; set; }

        [GraphQLField("population", "Float")]
        public virtual Single? Population { get; set; }

        [GraphQLField("climates", "[String]")]
        public virtual IEnumerable<String> Climates { get; set; }

        [GraphQLField("terrains", "[String]")]
        public virtual IEnumerable<String> Terrains { get; set; }

        [GraphQLField("surfaceWater", "Float")]
        public virtual Single? SurfaceWater { get; set; }

        [GraphQLField("residentConnection", "PlanetResidentsConnection")]
        public virtual PlanetResidentsConnection ResidentConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("residentConnection", "PlanetResidentsConnection")]
        public virtual PlanetResidentsConnection ResidentConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("residentConnection", "PlanetResidentsConnection")]
        public virtual PlanetResidentsConnection ResidentConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("residentConnection", "PlanetResidentsConnection")]
        public virtual PlanetResidentsConnection ResidentConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("residentConnection", "PlanetResidentsConnection")]
        public virtual PlanetResidentsConnection ResidentConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PlanetFilmsConnection")]
        public virtual PlanetFilmsConnection FilmConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PlanetFilmsConnection")]
        public virtual PlanetFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PlanetFilmsConnection")]
        public virtual PlanetFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PlanetFilmsConnection")]
        public virtual PlanetFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "PlanetFilmsConnection")]
        public virtual PlanetFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("created", "String")]
        public virtual String Created { get; set; }

        [GraphQLField("edited", "String")]
        public virtual String Edited { get; set; }

        [GraphQLField("id", "ID!")]
        public virtual String Id { get; set; }
    }

    [GraphQLType("PlanetFilmsConnection")]
    public class PlanetFilmsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[PlanetFilmsEdge]")]
        public virtual IEnumerable<PlanetFilmsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("films", "[Film]")]
        public virtual IEnumerable<Film> Films { get; set; }
    }

    [GraphQLType("PlanetFilmsEdge")]
    public class PlanetFilmsEdge
    {
        [GraphQLField("node", "Film")]
        public virtual Film Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("PlanetResidentsConnection")]
    public class PlanetResidentsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[PlanetResidentsEdge]")]
        public virtual IEnumerable<PlanetResidentsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("residents", "[Person]")]
        public virtual IEnumerable<Person> Residents { get; set; }
    }

    [GraphQLType("PlanetResidentsEdge")]
    public class PlanetResidentsEdge
    {
        [GraphQLField("node", "Person")]
        public virtual Person Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("PlanetsConnection")]
    public class PlanetsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[PlanetsEdge]")]
        public virtual IEnumerable<PlanetsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("planets", "[Planet]")]
        public virtual IEnumerable<Planet> Planets { get; set; }
    }

    [GraphQLType("PlanetsEdge")]
    public class PlanetsEdge
    {
        [GraphQLField("node", "Planet")]
        public virtual Planet Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("Root")]
    public class Root
    {
        [GraphQLField("allFilms", "FilmsConnection")]
        public virtual FilmsConnection AllFilms()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allFilms", "FilmsConnection")]
        public virtual FilmsConnection AllFilms([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allFilms", "FilmsConnection")]
        public virtual FilmsConnection AllFilms([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allFilms", "FilmsConnection")]
        public virtual FilmsConnection AllFilms([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allFilms", "FilmsConnection")]
        public virtual FilmsConnection AllFilms([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("film", "Film")]
        public virtual Film Film()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("film", "Film")]
        public virtual Film Film([GraphQLArgument("id", "ID")] String id)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("film", "Film")]
        public virtual Film Film([GraphQLArgument("id", "ID")] String id, [GraphQLArgument("filmID", "ID")] String filmID)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPeople", "PeopleConnection")]
        public virtual PeopleConnection AllPeople()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPeople", "PeopleConnection")]
        public virtual PeopleConnection AllPeople([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPeople", "PeopleConnection")]
        public virtual PeopleConnection AllPeople([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPeople", "PeopleConnection")]
        public virtual PeopleConnection AllPeople([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPeople", "PeopleConnection")]
        public virtual PeopleConnection AllPeople([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("person", "Person")]
        public virtual Person Person()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("person", "Person")]
        public virtual Person Person([GraphQLArgument("id", "ID")] String id)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("person", "Person")]
        public virtual Person Person([GraphQLArgument("id", "ID")] String id, [GraphQLArgument("personID", "ID")] String personID)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPlanets", "PlanetsConnection")]
        public virtual PlanetsConnection AllPlanets()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPlanets", "PlanetsConnection")]
        public virtual PlanetsConnection AllPlanets([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPlanets", "PlanetsConnection")]
        public virtual PlanetsConnection AllPlanets([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPlanets", "PlanetsConnection")]
        public virtual PlanetsConnection AllPlanets([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allPlanets", "PlanetsConnection")]
        public virtual PlanetsConnection AllPlanets([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("planet", "Planet")]
        public virtual Planet Planet()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("planet", "Planet")]
        public virtual Planet Planet([GraphQLArgument("id", "ID")] String id)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("planet", "Planet")]
        public virtual Planet Planet([GraphQLArgument("id", "ID")] String id, [GraphQLArgument("planetID", "ID")] String planetID)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allSpecies", "SpeciesConnection")]
        public virtual SpeciesConnection AllSpecies()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allSpecies", "SpeciesConnection")]
        public virtual SpeciesConnection AllSpecies([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allSpecies", "SpeciesConnection")]
        public virtual SpeciesConnection AllSpecies([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allSpecies", "SpeciesConnection")]
        public virtual SpeciesConnection AllSpecies([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allSpecies", "SpeciesConnection")]
        public virtual SpeciesConnection AllSpecies([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("species", "Species")]
        public virtual Species Species()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("species", "Species")]
        public virtual Species Species([GraphQLArgument("id", "ID")] String id)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("species", "Species")]
        public virtual Species Species([GraphQLArgument("id", "ID")] String id, [GraphQLArgument("speciesID", "ID")] String speciesID)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allStarships", "StarshipsConnection")]
        public virtual StarshipsConnection AllStarships()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allStarships", "StarshipsConnection")]
        public virtual StarshipsConnection AllStarships([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allStarships", "StarshipsConnection")]
        public virtual StarshipsConnection AllStarships([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allStarships", "StarshipsConnection")]
        public virtual StarshipsConnection AllStarships([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allStarships", "StarshipsConnection")]
        public virtual StarshipsConnection AllStarships([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starship", "Starship")]
        public virtual Starship Starship()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starship", "Starship")]
        public virtual Starship Starship([GraphQLArgument("id", "ID")] String id)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starship", "Starship")]
        public virtual Starship Starship([GraphQLArgument("id", "ID")] String id, [GraphQLArgument("starshipID", "ID")] String starshipID)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allVehicles", "VehiclesConnection")]
        public virtual VehiclesConnection AllVehicles()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allVehicles", "VehiclesConnection")]
        public virtual VehiclesConnection AllVehicles([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allVehicles", "VehiclesConnection")]
        public virtual VehiclesConnection AllVehicles([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allVehicles", "VehiclesConnection")]
        public virtual VehiclesConnection AllVehicles([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("allVehicles", "VehiclesConnection")]
        public virtual VehiclesConnection AllVehicles([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicle", "Vehicle")]
        public virtual Vehicle Vehicle()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicle", "Vehicle")]
        public virtual Vehicle Vehicle([GraphQLArgument("id", "ID")] String id)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("vehicle", "Vehicle")]
        public virtual Vehicle Vehicle([GraphQLArgument("id", "ID")] String id, [GraphQLArgument("vehicleID", "ID")] String vehicleID)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("node", "Node")]
        public virtual Node Node([GraphQLArgument("id", "ID!")] String id)
        {
            throw new InvalidOperationException();
        }
    }

    [GraphQLType("Species")]
    public class Species : Node
    {
        [GraphQLField("name", "String")]
        public virtual String Name { get; set; }

        [GraphQLField("classification", "String")]
        public virtual String Classification { get; set; }

        [GraphQLField("designation", "String")]
        public virtual String Designation { get; set; }

        [GraphQLField("averageHeight", "Float")]
        public virtual Single? AverageHeight { get; set; }

        [GraphQLField("averageLifespan", "Int")]
        public virtual Int32? AverageLifespan { get; set; }

        [GraphQLField("eyeColors", "[String]")]
        public virtual IEnumerable<String> EyeColors { get; set; }

        [GraphQLField("hairColors", "[String]")]
        public virtual IEnumerable<String> HairColors { get; set; }

        [GraphQLField("skinColors", "[String]")]
        public virtual IEnumerable<String> SkinColors { get; set; }

        [GraphQLField("language", "String")]
        public virtual String Language { get; set; }

        [GraphQLField("homeworld", "Planet")]
        public virtual Planet Homeworld { get; set; }

        [GraphQLField("personConnection", "SpeciesPeopleConnection")]
        public virtual SpeciesPeopleConnection PersonConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("personConnection", "SpeciesPeopleConnection")]
        public virtual SpeciesPeopleConnection PersonConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("personConnection", "SpeciesPeopleConnection")]
        public virtual SpeciesPeopleConnection PersonConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("personConnection", "SpeciesPeopleConnection")]
        public virtual SpeciesPeopleConnection PersonConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("personConnection", "SpeciesPeopleConnection")]
        public virtual SpeciesPeopleConnection PersonConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "SpeciesFilmsConnection")]
        public virtual SpeciesFilmsConnection FilmConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "SpeciesFilmsConnection")]
        public virtual SpeciesFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "SpeciesFilmsConnection")]
        public virtual SpeciesFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "SpeciesFilmsConnection")]
        public virtual SpeciesFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "SpeciesFilmsConnection")]
        public virtual SpeciesFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("created", "String")]
        public virtual String Created { get; set; }

        [GraphQLField("edited", "String")]
        public virtual String Edited { get; set; }

        [GraphQLField("id", "ID!")]
        public virtual String Id { get; set; }
    }

    [GraphQLType("SpeciesConnection")]
    public class SpeciesConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[SpeciesEdge]")]
        public virtual IEnumerable<SpeciesEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("species", "[Species]")]
        public virtual IEnumerable<Species> Species { get; set; }
    }

    [GraphQLType("SpeciesEdge")]
    public class SpeciesEdge
    {
        [GraphQLField("node", "Species")]
        public virtual Species Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("SpeciesFilmsConnection")]
    public class SpeciesFilmsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[SpeciesFilmsEdge]")]
        public virtual IEnumerable<SpeciesFilmsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("films", "[Film]")]
        public virtual IEnumerable<Film> Films { get; set; }
    }

    [GraphQLType("SpeciesFilmsEdge")]
    public class SpeciesFilmsEdge
    {
        [GraphQLField("node", "Film")]
        public virtual Film Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("SpeciesPeopleConnection")]
    public class SpeciesPeopleConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[SpeciesPeopleEdge]")]
        public virtual IEnumerable<SpeciesPeopleEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("people", "[Person]")]
        public virtual IEnumerable<Person> People { get; set; }
    }

    [GraphQLType("SpeciesPeopleEdge")]
    public class SpeciesPeopleEdge
    {
        [GraphQLField("node", "Person")]
        public virtual Person Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("Starship")]
    public class Starship : Node
    {
        [GraphQLField("name", "String")]
        public virtual String Name { get; set; }

        [GraphQLField("model", "String")]
        public virtual String Model { get; set; }

        [GraphQLField("starshipClass", "String")]
        public virtual String StarshipClass { get; set; }

        [GraphQLField("manufacturers", "[String]")]
        public virtual IEnumerable<String> Manufacturers { get; set; }

        [GraphQLField("costInCredits", "Float")]
        public virtual Single? CostInCredits { get; set; }

        [GraphQLField("length", "Float")]
        public virtual Single? Length { get; set; }

        [GraphQLField("crew", "String")]
        public virtual String Crew { get; set; }

        [GraphQLField("passengers", "String")]
        public virtual String Passengers { get; set; }

        [GraphQLField("maxAtmospheringSpeed", "Int")]
        public virtual Int32? MaxAtmospheringSpeed { get; set; }

        [GraphQLField("hyperdriveRating", "Float")]
        public virtual Single? HyperdriveRating { get; set; }

        [GraphQLField("MGLT", "Int")]
        public virtual Int32? Mglt { get; set; }

        [GraphQLField("cargoCapacity", "Float")]
        public virtual Single? CargoCapacity { get; set; }

        [GraphQLField("consumables", "String")]
        public virtual String Consumables { get; set; }

        [GraphQLField("pilotConnection", "StarshipPilotsConnection")]
        public virtual StarshipPilotsConnection PilotConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("pilotConnection", "StarshipPilotsConnection")]
        public virtual StarshipPilotsConnection PilotConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("pilotConnection", "StarshipPilotsConnection")]
        public virtual StarshipPilotsConnection PilotConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("pilotConnection", "StarshipPilotsConnection")]
        public virtual StarshipPilotsConnection PilotConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("pilotConnection", "StarshipPilotsConnection")]
        public virtual StarshipPilotsConnection PilotConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "StarshipFilmsConnection")]
        public virtual StarshipFilmsConnection FilmConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "StarshipFilmsConnection")]
        public virtual StarshipFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "StarshipFilmsConnection")]
        public virtual StarshipFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "StarshipFilmsConnection")]
        public virtual StarshipFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "StarshipFilmsConnection")]
        public virtual StarshipFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("created", "String")]
        public virtual String Created { get; set; }

        [GraphQLField("edited", "String")]
        public virtual String Edited { get; set; }

        [GraphQLField("id", "ID!")]
        public virtual String Id { get; set; }
    }

    [GraphQLType("StarshipFilmsConnection")]
    public class StarshipFilmsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[StarshipFilmsEdge]")]
        public virtual IEnumerable<StarshipFilmsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("films", "[Film]")]
        public virtual IEnumerable<Film> Films { get; set; }
    }

    [GraphQLType("StarshipFilmsEdge")]
    public class StarshipFilmsEdge
    {
        [GraphQLField("node", "Film")]
        public virtual Film Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("StarshipPilotsConnection")]
    public class StarshipPilotsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[StarshipPilotsEdge]")]
        public virtual IEnumerable<StarshipPilotsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("pilots", "[Person]")]
        public virtual IEnumerable<Person> Pilots { get; set; }
    }

    [GraphQLType("StarshipPilotsEdge")]
    public class StarshipPilotsEdge
    {
        [GraphQLField("node", "Person")]
        public virtual Person Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("StarshipsConnection")]
    public class StarshipsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[StarshipsEdge]")]
        public virtual IEnumerable<StarshipsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("starships", "[Starship]")]
        public virtual IEnumerable<Starship> Starships { get; set; }
    }

    [GraphQLType("StarshipsEdge")]
    public class StarshipsEdge
    {
        [GraphQLField("node", "Starship")]
        public virtual Starship Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("Vehicle")]
    public class Vehicle : Node
    {
        [GraphQLField("name", "String")]
        public virtual String Name { get; set; }

        [GraphQLField("model", "String")]
        public virtual String Model { get; set; }

        [GraphQLField("vehicleClass", "String")]
        public virtual String VehicleClass { get; set; }

        [GraphQLField("manufacturers", "[String]")]
        public virtual IEnumerable<String> Manufacturers { get; set; }

        [GraphQLField("costInCredits", "Float")]
        public virtual Single? CostInCredits { get; set; }

        [GraphQLField("length", "Float")]
        public virtual Single? Length { get; set; }

        [GraphQLField("crew", "String")]
        public virtual String Crew { get; set; }

        [GraphQLField("passengers", "String")]
        public virtual String Passengers { get; set; }

        [GraphQLField("maxAtmospheringSpeed", "Int")]
        public virtual Int32? MaxAtmospheringSpeed { get; set; }

        [GraphQLField("cargoCapacity", "Float")]
        public virtual Single? CargoCapacity { get; set; }

        [GraphQLField("consumables", "String")]
        public virtual String Consumables { get; set; }

        [GraphQLField("pilotConnection", "VehiclePilotsConnection")]
        public virtual VehiclePilotsConnection PilotConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("pilotConnection", "VehiclePilotsConnection")]
        public virtual VehiclePilotsConnection PilotConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("pilotConnection", "VehiclePilotsConnection")]
        public virtual VehiclePilotsConnection PilotConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("pilotConnection", "VehiclePilotsConnection")]
        public virtual VehiclePilotsConnection PilotConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("pilotConnection", "VehiclePilotsConnection")]
        public virtual VehiclePilotsConnection PilotConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "VehicleFilmsConnection")]
        public virtual VehicleFilmsConnection FilmConnection()
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "VehicleFilmsConnection")]
        public virtual VehicleFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "VehicleFilmsConnection")]
        public virtual VehicleFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "VehicleFilmsConnection")]
        public virtual VehicleFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("filmConnection", "VehicleFilmsConnection")]
        public virtual VehicleFilmsConnection FilmConnection([GraphQLArgument("after", "String")] String after, [GraphQLArgument("first", "Int")] Int32? first, [GraphQLArgument("before", "String")] String before, [GraphQLArgument("last", "Int")] Int32? last)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("created", "String")]
        public virtual String Created { get; set; }

        [GraphQLField("edited", "String")]
        public virtual String Edited { get; set; }

        [GraphQLField("id", "ID!")]
        public virtual String Id { get; set; }
    }

    [GraphQLType("VehicleFilmsConnection")]
    public class VehicleFilmsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[VehicleFilmsEdge]")]
        public virtual IEnumerable<VehicleFilmsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("films", "[Film]")]
        public virtual IEnumerable<Film> Films { get; set; }
    }

    [GraphQLType("VehicleFilmsEdge")]
    public class VehicleFilmsEdge
    {
        [GraphQLField("node", "Film")]
        public virtual Film Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("VehiclePilotsConnection")]
    public class VehiclePilotsConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[VehiclePilotsEdge]")]
        public virtual IEnumerable<VehiclePilotsEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("pilots", "[Person]")]
        public virtual IEnumerable<Person> Pilots { get; set; }
    }

    [GraphQLType("VehiclePilotsEdge")]
    public class VehiclePilotsEdge
    {
        [GraphQLField("node", "Person")]
        public virtual Person Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }

    [GraphQLType("VehiclesConnection")]
    public class VehiclesConnection
    {
        [GraphQLField("pageInfo", "PageInfo!")]
        public virtual PageInfo PageInfo { get; set; }

        [GraphQLField("edges", "[VehiclesEdge]")]
        public virtual IEnumerable<VehiclesEdge> Edges { get; set; }

        [GraphQLField("totalCount", "Int")]
        public virtual Int32? TotalCount { get; set; }

        [GraphQLField("vehicles", "[Vehicle]")]
        public virtual IEnumerable<Vehicle> Vehicles { get; set; }
    }

    [GraphQLType("VehiclesEdge")]
    public class VehiclesEdge
    {
        [GraphQLField("node", "Vehicle")]
        public virtual Vehicle Node { get; set; }

        [GraphQLField("cursor", "String!")]
        public virtual String Cursor { get; set; }
    }
}