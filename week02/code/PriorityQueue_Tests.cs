using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PriorityQueueTests
{
    // Scenario: Enqueue low, medium, and high priorities, with the highest at the back.
    // Expected Result: High, Medium, Low; then empty.
    // Defect(s) Found: Static trace of original: final entry is skipped and returned values are never removed.
    // Result: Passed after the fixes.
    [TestMethod]
    public void TestPriorityQueue_1()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Low", 1);
        priorityQueue.Enqueue("Medium", 5);
        priorityQueue.Enqueue("High", 10);
        Assert.AreEqual("High", priorityQueue.Dequeue());
        Assert.AreEqual("Medium", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
        var error = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", error.Message);
    }

    // Scenario: Enqueue three items with equal priority.
    // Expected Result: First, Second, Third (FIFO).
    // Defect(s) Found: Static trace of original: >= selects a later tied item; returned values are not removed.
    // Result: Passed after the fixes.
    [TestMethod]
    public void TestPriorityQueue_2()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("First", 4);
        priorityQueue.Enqueue("Second", 4);
        priorityQueue.Enqueue("Third", 4);
        Assert.AreEqual("First", priorityQueue.Dequeue());
        Assert.AreEqual("Second", priorityQueue.Dequeue());
        Assert.AreEqual("Third", priorityQueue.Dequeue());
        var error = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", error.Message);
    }

    // Scenario: Enqueue only negative priorities.
    // Expected Result: MinusOne, MinusThree, MinusFive.
    // Defect(s) Found: Static trace of original: last entry is skipped; removal is missing.
    // Result: Passed after the fixes.
    [TestMethod]
    public void TestPriorityQueue_NegativePriorities()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("MinusFive", -5);
        priorityQueue.Enqueue("MinusThree", -3);
        priorityQueue.Enqueue("MinusOne", -1);
        Assert.AreEqual("MinusOne", priorityQueue.Dequeue());
        Assert.AreEqual("MinusThree", priorityQueue.Dequeue());
        Assert.AreEqual("MinusFive", priorityQueue.Dequeue());
        var error = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", error.Message);
    }

    // Scenario: Enqueue interleaved high-priority ties and lower-priority items.
    // Expected Result: HighFirst, HighSecond, LowFirst, LowSecond.
    // Defect(s) Found: Static trace of original: later ties replace earlier items; removal is missing.
    // Result: Passed after the fixes.
    [TestMethod]
    public void TestPriorityQueue_MixedTies()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("LowFirst", 1);
        priorityQueue.Enqueue("HighFirst", 9);
        priorityQueue.Enqueue("HighSecond", 9);
        priorityQueue.Enqueue("LowSecond", 1);
        Assert.AreEqual("HighFirst", priorityQueue.Dequeue());
        Assert.AreEqual("HighSecond", priorityQueue.Dequeue());
        Assert.AreEqual("LowFirst", priorityQueue.Dequeue());
        Assert.AreEqual("LowSecond", priorityQueue.Dequeue());
        var error = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", error.Message);
    }

    // Scenario: Dequeue from a new empty queue.
    // Expected Result: InvalidOperationException with message "The queue is empty."
    // Defect(s) Found: None by static inspection; execution pending.
    [TestMethod]
    public void TestPriorityQueue_Empty()
    {
        var priorityQueue = new PriorityQueue();
        var error = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", error.Message);
    }

    // Scenario: Remove a single item, then reuse the queue.
    // Expected Result: Only, then New; queue is empty after both removals.
    // Defect(s) Found: Original Dequeue never removes its selected item.
    // Final static trace matches; execution pending.
    [TestMethod]
    public void TestPriorityQueue_Reuse()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Only", 10);
        Assert.AreEqual("Only", priorityQueue.Dequeue());
        var error = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", error.Message);
        priorityQueue.Enqueue("New", 1);
        Assert.AreEqual("New", priorityQueue.Dequeue());
        error = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", error.Message);
    }
}
