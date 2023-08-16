﻿using BusinessLayer.Interfaces;
using CommonLayer.Model;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services
{
    public class CollabBusiness : ICollabBusiness
    {
        private readonly ICollabRepo collabRepo;
        public CollabBusiness(ICollabRepo collabRepo)
        {
            this.collabRepo = collabRepo;
        }


        public CollabEntity CreateCollab(CollabCreateModel model, long UserID, long NoteID)
        {
            try
            {
                return collabRepo.CreateCollab(model, UserID, NoteID);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


    }
}