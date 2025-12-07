@ui @login
Feature: Login Functionality
  As a user
  I want to be able to login to the application
  So that I can access my account

  Background:
    Given I am on the login page

  @smoke @positive
  Scenario: Successful login with valid credentials
    When I enter username "testuser"
    And I enter password "testpassword"
    And I click the login button
    Then I should be logged in successfully
    And I should see the home page

  @smoke @positive
  Scenario: Login with environment credentials
    When I login with valid credentials
    Then I should be logged in successfully

  @negative
  Scenario: Login with invalid username
    When I enter username "invaliduser"
    And I enter password "testpassword"
    And I click the login button
    Then I should see an error message

  @negative
  Scenario: Login with invalid password
    When I enter username "testuser"
    And I enter password "wrongpassword"
    And I click the login button
    Then I should see an error message

  @negative
  Scenario: Login with empty credentials
    When I click the login button
    Then I should see an error message

  @positive
  Scenario Outline: Login with multiple users
    When I enter username "<username>"
    And I enter password "<password>"
    And I click the login button
    Then I should be logged in successfully

    Examples:
      | username  | password     |
      | user1     | password1    |
      | user2     | password2    |
      | admin     | adminpass    |

  @positive
  Scenario: Login with remember me option
    When I enter username "testuser"
    And I enter password "testpassword"
    And I check the remember me checkbox
    And I click the login button
    Then I should be logged in successfully
