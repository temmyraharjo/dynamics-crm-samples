using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CrmDeploymentTests
{
    public class DatabaseWrapper : IOrganizationService
    {
        private readonly IOrganizationService _service;
        public List<Entity> CreatedEntities = new List<Entity>();
        public List<Entity> UpdatedEntities = new List<Entity>();
        public List<EntityReference> DeletedEntities = new List<EntityReference>();

        public DatabaseWrapper(IOrganizationService service)
        {
            _service = service;
        }

        public Guid Create(Entity entity)
        {
            var result = _service.Create(entity);
            entity.Id = result;
            CreatedEntities.Add(entity);

            return result;
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return _service.Retrieve(entityName, id, columnSet);
        }

        public void Update(Entity entity)
        {
            _service.Update(entity);
            UpdatedEntities.Add(entity);
        }

        public void Delete(string entityName, Guid id)
        {
            _service.Delete(entityName, id);
            DeletedEntities.Add(new EntityReference(entityName, id));
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return _service.Execute(request);
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _service.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship,
            EntityReferenceCollection relatedEntities)
        {
            _service.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return _service.RetrieveMultiple(query);
        }
    }
}