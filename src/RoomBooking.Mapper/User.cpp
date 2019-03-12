template <>
struct mapper<RoomBooking::User>
{
    static M::Entity on_create()
    {
        auto system = Name.ensure_string(LOCAL_SYSTEM_NAME).ensure_path_to_subject(HaveRelation, System);
        return UserRelation.create_with().subject(Agent.create()).object(system).go();
    }

    static void on_update_Username(entity root, const option<string> &value)
    {
        if (value)
            root.ensure_subject_of(HaveRelation, Name.ensure_string(*value));
        else
            root.ensure_not_subject_of(HaveRelation, Name);
    }

    static void on_update_Email(entity root, const option<string> &value)
    {
        if (value)
            root.ensure_subject_of(HaveRelation, EmailAddress.ensure_string(*value));
        else
            root.ensure_not_subject_of(HaveRelation, EmailAddress);
    }

    static void on_delete(entity root)
    {
        root.untrack();
    }

    static void invalidate(change_type type, const entity &obj, const entity::changes &changes, entity::invset &inval)
    {
        const auto F = obj.features() + (changes.features_assigned() ? changes.features() : featureset{});

        if (UserRelation.test(F))
        {
            inval(obj);
        }

        if (Agent.test(F))
        {
            from(obj).subject_of(UserRelation).for_each(inval);
        }

        if (System.test(F))
        {
            from(obj).object_of(UserRelation).for_each(inval);
        }

        if (HaveRelation.test(F))
        {
            from(obj.subject()).filter(UserRelation).for_each(inval);
            from(changes.subject_assigned() ? changes.subject() : none).filter(UserRelation).for_each(inval);
            from(obj.subject()).filter(System).object_of(UserRelation).for_each(inval);
            from(changes.subject_assigned() ? changes.subject() : none).filter(System).object_of(UserRelation).for_each(inval);
        }

        if (Name.test(F))
        {
            from(obj).object_of(HaveRelation).having_subject(UserRelation).for_each(inval);
            from(obj).object_of(HaveRelation).having_subject(System).object_of(UserRelation).for_each(inval);
        }

        if (EmailAddress.test(F))
        {
            from(obj).object_of(HaveRelation).having_subject(UserRelation).for_each(inval);
        }
    }

    static bool refresh(const entity &root, RoomBooking::User::refresh_batch &changes)
    {
        if (!UserRelation.test(root))
        {
            return false;
        }

        const auto subject = root.subject();
        const auto object = root.object();

        if (subject && subject->exists() && !Agent.test(*subject))
        {
            return false;
        }

        if (!from(root).having_object(System).subject_of(HaveRelation).having_object(Name).filter(LOCAL_SYSTEM_NAME).first().has_value())
        {
            return false;
        }

        const auto username = from(root).subject_of(HaveRelation).having_object(Name).first();
        const auto email = from(root).subject_of(HaveRelation).having_object(EmailAddress).first();

        changes.Username(username ? username->string() : none);
        changes.Email(email ? email->string() : none);

        return true;
    }
};
