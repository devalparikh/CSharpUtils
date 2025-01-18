// See https://aka.ms/new-console-template for more information

using EventDriven;
using EventDriven.Common;

// var topic1 = new PubSub();
var topic1 = new ReaderWriterPubSub();

var publisher1 = new Publisher("1");
var publisher2 = new Publisher("2");

var subscriber1 = new Subscriber("1");
var subscriber2 = new Subscriber("2");
var subscriber3 = new Subscriber("3");
subscriber1.Subscribe(topic1);
subscriber2.Subscribe(topic1);
subscriber3.Subscribe(topic1);

await publisher1.Publish(topic1, new Message("hi there"));
await publisher2.Publish(topic1, new Message("hi there as well"));