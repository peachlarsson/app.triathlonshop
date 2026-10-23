import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

param sqlServerName string

resource existingSqlServer 'Microsoft.Sql/servers@2026-08-01-preview' existing = {
  name: sqlServerName
} 

resource sqlElasticPool 'Microsoft.Sql/servers/elasticPools@2026-08-01-preview' = {
  name: 'sqlpool-${system}-${env}-${con.companyInfo.name}'
  parent: existingSqlServer
  location: con.location
  tags: tags
}

@secure()
output sqlElasticPoolId string = sqlElasticPool.id
