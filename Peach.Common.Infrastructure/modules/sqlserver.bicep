import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

resource sqlServer 'Microsoft.Sql/servers@2026-08-01-preview' = {
  name: 'sql-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
  properties: {
    
  }
}

output sqlServerName string = sqlServer.name
