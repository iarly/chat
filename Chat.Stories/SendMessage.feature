Feature: Send a message
	In order to met new friends
	As an user of the Chat
	I want to send a message to someone

Scenario: I like to send a message for everybody in the chat room
	Given I've connected to the chat server
	And I've set my nickname as 'Joseph'
	And I've connected to 'Best Fries Forever' chat room
	And 'Maria' has connected to chat room too
	And 'Sylvia' has connected to chat room too
	And 'Fernando' has connected to another chat room
	When I send a message like 'Hello Little World' for everybody
	Then I must receive: 'Joseph to everybody: Hello Little World'
	And 'Maria' must receive: 'Joseph to everybody: Hello Little World'
	And 'Sylvia' must receive: 'Joseph to everybody: Hello Little World'
	And 'Fernando' mustn't receive any message

Scenario: I like to send a public and direct message for Sylvia in the chat room
	Given I've connected to the chat server
	And I've set my nickname as 'Joseph'
	And I've connected to 'Best Fries Forever' chat room
	And 'Maria' has connected to chat room too
	And 'Sylvia' has connected to chat room too
	And 'Fernando' has connected to another chat room
	When I send a message like 'Hello Little Sunshine' for 'Sylvia'
	Then I must receive: 'Joseph to Sylvia: Hello Little Sunshine'
	And 'Maria' must receive: 'Joseph to Sylvia: Hello Little Sunshine'
	And 'Sylvia' must receive: 'Joseph to Sylvia: Hello Little Sunshine'
	And 'Fernando' mustn't receive my message

Scenario: I like to send a private message for Sylvia in the chat room
	Given I've connected to the chat server
	And I've set my nickname as 'Joseph'
	And I've connected to 'Best Fries Forever' chat room
	And 'Maria' has connected to chat room too
	And 'Sylvia' has connected to chat room too
	And 'Fernando' has connected to another chat room
	When I send a message like 'Hello My Love' for 'Sylvia'
	Then I must receive: 'Joseph secretly to Sylvia: Hello My Love'
	And 'Maria' mustn't receive my message
	And 'Sylvia' must receive: 'Joseph secretly to Sylvia: Hello My Love'
	And 'Fernando' mustn't receive my message