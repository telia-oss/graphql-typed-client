namespace Telia.GraphQL.Client.Example
{
    using System;
    using System.Collections.Generic;
    using Telia.GraphQL.Client.Attributes;

    public class Query
    {
        [GraphQLField("hero")]
        public Human Hero(Episode episode)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("reviews")]
        public IEnumerable<Review> Reviews(Episode episode)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("human")]
        public Human Human(String id)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("starship")]
        public Starship Starship(String id)
        {
            throw new InvalidOperationException();
        }
    }

    public enum Episode
    {
        NEWHOPE,
        EMPIRE,
        JEDI
    }

    public enum LengthUnit
    {
        METER,
        FOOT
    }

    public class Human
    {
        [GraphQLField("id")]
        public String Id
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

        [GraphQLField("height")]
        public Single? Height(LengthUnit unit)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("mass")]
        public Single? Mass
        {
            get;
            set;
        }

        [GraphQLField("friends")]
        public IEnumerable<Human> Friends
        {
            get;
            set;
        }

        [GraphQLField("friendsConnection")]
        public FriendsConnection FriendsConnection(Int32? first, String after)
        {
            throw new InvalidOperationException();
        }

        [GraphQLField("appearsIn")]
        public IEnumerable<Episode> AppearsIn
        {
            get;
            set;
        }

        [GraphQLField("starships")]
        public IEnumerable<Starship> Starships
        {
            get;
            set;
        }
    }

    public class FriendsConnection
    {
        [GraphQLField("totalCount")]
        public Int32? TotalCount
        {
            get;
            set;
        }

        [GraphQLField("edges")]
        public IEnumerable<FriendsEdge> Edges
        {
            get;
            set;
        }

        [GraphQLField("friends")]
        public IEnumerable<Human> Friends
        {
            get;
            set;
        }

        [GraphQLField("pageInfo")]
        public PageInfo PageInfo
        {
            get;
            set;
        }
    }

    public class FriendsEdge
    {
        [GraphQLField("cursor")]
        public String Cursor
        {
            get;
            set;
        }

        [GraphQLField("node")]
        public Human Node
        {
            get;
            set;
        }
    }

    public class PageInfo
    {
        [GraphQLField("startCursor")]
        public String StartCursor
        {
            get;
            set;
        }

        [GraphQLField("endCursor")]
        public String EndCursor
        {
            get;
            set;
        }

        [GraphQLField("hasNextPage")]
        public Boolean HasNextPage
        {
            get;
            set;
        }
    }

    public class Review
    {
        [GraphQLField("stars")]
        public Int32 Stars
        {
            get;
            set;
        }

        [GraphQLField("commentary")]
        public String Commentary
        {
            get;
            set;
        }
    }

    public class Starship
    {
        [GraphQLField("id")]
        public String Id
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

        [GraphQLField("length")]
        public Single? Length(LengthUnit unit)
        {
            throw new InvalidOperationException();
        }
    }
}