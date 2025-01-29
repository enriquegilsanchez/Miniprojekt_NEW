using System.Collections.Generic;

public abstract class Node
{
    public abstract NodeState Evaluate();
}

public enum NodeState
{
    Success,
    Failure,
    Running,
}

public class Selector : Node
{
    private List<Node> nodes = new List<Node>();

    public Selector(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            NodeState state = node.Evaluate();
            if (state == NodeState.Success)
                return NodeState.Success;
            if (state == NodeState.Running)
                return NodeState.Running;
        }
        return NodeState.Failure;
    }
}

public class Sequence : Node
{
    private List<Node> nodes = new List<Node>();

    public Sequence(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            NodeState state = node.Evaluate();
            if (state == NodeState.Failure)
                return NodeState.Failure;
            if (state == NodeState.Running)
                return NodeState.Running;
        }
        return NodeState.Success;
    }
}
