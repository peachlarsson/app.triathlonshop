import * as cmn from './common.bicep'

param env cmn.env
param system string
param tags cmn.tags = {
  Creator: cmn.creator
  Environment: env
}

resource sqldb 'Microsoft.Sql/servers@2025-02-01-preview' = {
  name: 'sqldb-${system}-${env}-${cmn.companyInfo.name}'
  location: cmn.location
  tags: tags
}
