@api
Feature: REST API Testing
  As a developer
  I want to test REST API endpoints
  So that I can ensure the API works correctly

  Background:
    Given I have an API client

  @smoke @get
  Scenario: Get list of users
    When I send a GET request to "/users"
    Then the response status code should be 200
    And the response should be successful

  @get
  Scenario: Get user by ID
    When I send a GET request to "/users/1"
    Then the response status code should be 200
    And the response JSON should have property "id"
    And the response JSON property "id" should be "1"

  @get
  Scenario: Get users with query parameters
    When I send a GET request to "/users" with query parameters
      | Key    | Value |
      | page   | 1     |
      | limit  | 10    |
    Then the response should be successful

  @post
  Scenario: Create a new user
    When I send a POST request to "/users" with JSON
      | Key       | Value              |
      | name      | John Doe           |
      | email     | john@example.com   |
      | username  | johndoe            |
    Then the response status code should be 201
    And the response JSON should have property "id"

  @put
  Scenario: Update an existing user
    When I send a PUT request to "/users/1" with body
      """
      {
        "name": "John Updated",
        "email": "john.updated@example.com"
      }
      """
    Then the response status code should be 200

  @delete
  Scenario: Delete a user
    When I send a DELETE request to "/users/1"
    Then the response status code should be 200

  @negative
  Scenario: Get non-existent user returns 404
    When I send a GET request to "/users/99999"
    Then the response status code should be 404

  @authentication
  Scenario: API request with authentication
    Given I set the bearer token "test-token-123"
    When I send a GET request to "/protected/resource"
    Then the response should be successful

  @chained
  Scenario: Create and retrieve user
    When I send a POST request to "/users" with JSON
      | Key   | Value         |
      | name  | Test User     |
      | email | test@test.com |
    Then the response status code should be 201
    And I store response property "id" as "userId"
