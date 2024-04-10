Feature: Sequence
    In order to create unique ID's
    As a programmer
    I want to use a sequence

Scenario: Create a new sequence
    Given an empty database was created
    When I create a sequence
    Then the next value in that sequence should be 1

Scenario: Get values from a sequence
    Given an empty database was created
    And a sequence was created
    When I get one value from that sequence
    And I open the database later
    Then the next value in that sequence should be 2
