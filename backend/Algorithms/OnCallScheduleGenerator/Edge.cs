namespace MedicalDemo.Algorithms.OnCallScheduleGenerator;

public class Edge
{
    public int currentCap, originalCap; // days

    public int
        destination, reverse; // destination is node, reverse is the index

    public Edge(int destination, int reverse, int cap)
    {
        this.destination = destination;
        this.reverse = reverse;
        currentCap = cap;
        originalCap = cap;
    }

    public int flow()
    {
        return
            originalCap -
            currentCap; // allow flows to be negative for finding reverse edge flows
        // return Math.Max(originalCap-currentCap, 0);
    }
}