import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

param sqlServerName string

@secure()
param sqlElasticPoolId string

param sku object = {
  name: 'GP_S_Gen5_2'
  tier: 'GeneralPurpose'
  family: 'Gen5'
  capacity: 2
}

resource existingSqlServer 'Microsoft.Sql/servers@2026-08-01-preview' existing = {
  name: sqlServerName
}

resource sqldb 'Microsoft.Sql/servers/databases@2025-02-01-preview' = {
  parent: existingSqlServer
  name: 'sqldb-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
  sku: sku
  properties: {
    elasticPoolId: sqlElasticPoolId
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368
    zoneRedundant: false
    readScale: 'Disabled'
    autoPauseDelay: 60
    requestedBackupStorageRedundancy: 'Local'
    minCapacity: json('0.5')
    isLedgerOn: false
    useFreeLimit: true
    freeLimitExhaustionBehavior: 'BillOverUsage'
    availabilityZone: 'NoPreference'
  }
}
