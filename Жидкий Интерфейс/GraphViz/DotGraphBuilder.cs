using System;
using System.Globalization;
using System.Linq;

namespace FluentApi.Graph
{
    public enum NodeShape
    {
        Box,
        Ellipse
    }
	
	public class DotGraphBuilder
    {
        protected readonly Graph _graph;

		protected DotGraphBuilder(Graph graph) => _graph = graph;

		public static DotGraphBuilder DirectedGraph(string graphName) => 
            new DotGraphBuilder(new Graph(graphName, true, false));

        public static DotGraphBuilder UndirectedGraph(string graphName) =>
            new DotGraphBuilder(new Graph(graphName, false, false));

        public string Build() => _graph.ToDotFormat();

        public NodeWithBuilder AddNode(string nodeName)
        {
            _graph.AddNode(nodeName);
            return new NodeWithBuilder(_graph);
        }

        public EdgeWithBuilder AddEdge(string sourceNode, string destinationNode)
        {
            _graph.AddEdge(sourceNode, destinationNode);
            return new EdgeWithBuilder(_graph);
        }
    }

    public class EdgeWithBuilder : DotGraphBuilder
    {
        public EdgeWithBuilder(Graph graph) : base(graph) { }

        public DotGraphBuilder With(Action<object> descript)
        {
            descript(_graph.Edges.Last());
            return this;
        }
    }

    public class NodeWithBuilder : DotGraphBuilder
    {
        public NodeWithBuilder(Graph graph) : base(graph) { }

        public DotGraphBuilder With(Action<object> descript)
        {
            descript(_graph.Nodes.Last());
            return this;
        }
    }

    public static class NodeUpdater
    {
        public static object Label(this object graphObj, string label)
        {
            switch (graphObj)
            {
                case GraphNode graphNode:
                    graphNode.Attributes.Add(nameof(label), label);
                    return graphObj;
                case GraphEdge graphEdge:
                    graphEdge.Attributes.Add(nameof(label), label);
                    return graphObj;
                default:
                    return null;
            }
		}

        public static object FontSize(this object graphObj, int fontsize)
        {
            switch (graphObj)
            {
                case GraphNode graphNode:
                    graphNode.Attributes.Add(nameof(fontsize), fontsize.ToString());
                    return graphObj;
                case GraphEdge graphEdge:
                    graphEdge.Attributes.Add(nameof(fontsize), fontsize.ToString());
                    return graphObj;
                default:
                    return null;
            }
		}

        public static object Weight(this object graphObj, double weight)
        {
            switch (graphObj)
            {
                case GraphNode graphNode:
                    graphNode.Attributes.Add(nameof(weight), weight.ToString(CultureInfo.InvariantCulture));
                    return graphObj;
                case GraphEdge graphEdge:
                    graphEdge.Attributes.Add(nameof(weight), weight.ToString(CultureInfo.InvariantCulture));
                    return graphObj;
                default:
                    return null;
            }
		}

        public static object Shape(this object graphObj, NodeShape shape)
        {
            switch (graphObj)
            {
                case GraphNode graphNode:
                    graphNode.Attributes.Add(nameof(shape), shape.ToString().ToLower());
                    return graphObj;
                case GraphEdge graphEdge:
                    graphEdge.Attributes.Add(nameof(shape), shape.ToString().ToLower());
                    return graphObj;
                default:
                    return null;
            }
		}

        public static object Color(this object graphObj, string color)
        {
            switch (graphObj)
            {
                case GraphNode graphNode:
                    graphNode.Attributes.Add(nameof(color), color);
                    return graphObj;
                case GraphEdge graphEdge:
                    graphEdge.Attributes.Add(nameof(color), color);
                    return graphObj;
                default:
                    return null;
            }
        }
    }
}