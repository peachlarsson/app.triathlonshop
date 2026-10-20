@export()
type env = 'dev' | 'stg' | 'prd'

@export()
type creatorType = 'bicep'

@export()
var creator = 'bicep'

@export()
type tags = {
  Environment: env
  Creator: creatorType
}

@export()
var location = resourceGroup().location

@export()
type netFrameworkVersionType = 'v10.0' | 'v8.0'

@export()
var companyInfo = {
  name: 'peach'
}

@export()
var environment = {
  long: {
    camelCase: {
      dev: 'development'
      stg: 'staging'
      prd: 'production'
    }
    capitalizedFirstLetter: {
      dev: 'Development'
      stg: 'Staging'
      prd: 'Production'
    }
  }
}

@export()
var subscriptionIds = {
  'Volvobil DevTest': '2e822ced-6a74-4dc7-b47f-6144e1c0f892'
  'Volvobil Prod': '1eda972d-1081-4322-aa7a-4b9638c62feb'
}
