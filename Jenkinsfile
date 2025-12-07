pipeline {
    agent any
    
    parameters {
        choice(
            name: 'ENVIRONMENT',
            choices: ['Development', 'Staging', 'Production'],
            description: 'Test Environment'
        )
        choice(
            name: 'BROWSER',
            choices: ['Chrome', 'Firefox', 'Edge'],
            description: 'Browser for UI tests'
        )
        string(
            name: 'TAGS',
            defaultValue: '',
            description: 'Test tags to run (e.g., @smoke, @regression)'
        )
        booleanParam(
            name: 'HEADLESS',
            defaultValue: true,
            description: 'Run browser in headless mode'
        )
    }
    
    environment {
        DOTNET_VERSION = '10.0'
        TEST_ENVIRONMENT = "${params.ENVIRONMENT}"
        BROWSER_TYPE = "${params.BROWSER}"
        BROWSER_HEADLESS = "${params.HEADLESS}"
        // Credentials from Jenkins credentials store
        APP_BASE_URL = credentials('app-base-url')
        API_BASE_URL = credentials('api-base-url')
        TEST_USERNAME = credentials('test-username')
        TEST_PASSWORD = credentials('test-password')
        API_KEY = credentials('api-key')
    }
    
    tools {
        dotnetsdk 'dotnet-10'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Restore Dependencies') {
            steps {
                sh 'dotnet restore'
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build --no-restore --configuration Release'
            }
        }
        
        stage('Run Tests') {
            steps {
                script {
                    def filterArg = ''
                    if (params.TAGS?.trim()) {
                        filterArg = "--filter \"Category=${params.TAGS}\""
                    }
                    
                    sh """
                        dotnet test --no-build --configuration Release \
                            ${filterArg} \
                            --logger "trx;LogFileName=test-results.trx" \
                            --logger "html;LogFileName=test-results.html" \
                            --results-directory ./TestResults
                    """
                }
            }
            post {
                always {
                    // Archive test results
                    archiveArtifacts artifacts: 'TestResults/**/*', allowEmptyArchive: true
                    archiveArtifacts artifacts: 'Reports/**/*', allowEmptyArchive: true
                    
                    // Publish test results
                    mstest testResultsFile: 'TestResults/*.trx', keepLongStdio: true
                }
            }
        }
        
        stage('Generate Allure Report') {
            steps {
                allure([
                    includeProperties: false,
                    jdk: '',
                    properties: [],
                    reportBuildPolicy: 'ALWAYS',
                    results: [[path: 'allure-results']]
                ])
            }
        }
    }
    
    post {
        always {
            // Clean workspace
            cleanWs()
        }
        success {
            echo 'Tests passed successfully!'
        }
        failure {
            echo 'Tests failed!'
            // Archive screenshots on failure
            archiveArtifacts artifacts: 'Reports/Screenshots/**/*', allowEmptyArchive: true
        }
    }
}
