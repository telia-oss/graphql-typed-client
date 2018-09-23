namespace GraphQLTypedClient.Example.Schema
{
    using System;
    using System.Collections.Generic;
    using GraphQLTypedClient.ClassGenerator.Attributes;

    public enum CharacterRole
    {
        Main,
        Side
    }

    public class Actor
    {
        [GraphQLField("name")]
        public String Name
        {
            get;
            set;
        }

        [GraphQLField("age")]
        public Int32 Age
        {
            get;
            set;
        }

        [GraphQLField("characters")]
        public IEnumerable<MovieCharacter> Characters
        {
            get;
            set;
        }
    }

    public class MovieCharacter
    {
        [GraphQLField("actor")]
        public Actor Actor
        {
            get;
            set;
        }

        [GraphQLField("name")]
        public String Name
        {
            get;
            set;
        }

        [GraphQLField("movie")]
        public Movie Movie
        {
            get;
            set;
        }

        [GraphQLField("role")]
        public CharacterRole Role
        {
            get;
            set;
        }
    }

    public class Movie
    {
        [GraphQLField("name")]
        public String Name
        {
            get;
            set;
        }

        [GraphQLField("characters")]
        public IEnumerable<MovieCharacter> Characters
        {
            get;
            set;
        }

        [GraphQLField("released")]
        public DateTime? Released
        {
            get;
            set;
        }
    }

    public class Query
    {
        [GraphQLField("actors")]
        public IEnumerable<Actor> Actors
        {
            get;
            set;
        }

        [GraphQLField("actor")]
        public Actor Actor(String ID)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("movies")]
        public IEnumerable<Movie> Movies
        {
            get;
            set;
        }

        [GraphQLField("movie")]
        public Movie Movie
        {
            get;
            set;
        }
    }
}