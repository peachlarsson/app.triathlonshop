import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

resource sqldb 'Microsoft.Sql/servers@2025-02-01-preview' = {
  name: 'sqldb-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
}
