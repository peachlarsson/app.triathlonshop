import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

resource sqlElasticPool 'Microsoft.Sql/servers/elasticPools@2026-08-01-preview' = {
  name: 'sqlpool-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
}
