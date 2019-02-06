template <>
struct mapper<RoomBooking::UserSession>
{
    static M::Entity on_create()
    {
        return UserAccountSession.create_with().start(datetime::now()).go();
    }

    static void on_update_User(entity root, const option<RoomBooking::User> &value)
    {
        from(root).object_of(HaveRelation).filter_subject(UserRelation).for_each([](entity e) { e.destroy(); });
        if (value)
        {
            root.ensure_object_of(HaveRelation, entity::from(*value));
        }
    }

    static void on_update_SessionId(entity root, const option<string> &value)
    {
        if (value)
            root.ensure_subject_of(HaveRelation, Identifier.ensure_string(*value));
        else
            root.ensure_not_subject_of(HaveRelation, Identifier);
    }

    static void on_update_ExpiresAt(entity root, const datetime &value)
    {
        root.end(value != datetime::min_value() ? some(value) : none);
    }

    static void on_delete(entity root)
    {
        root.untrack();
    }

    static void invalidate(change_type type, const entity &obj, const entity::changes &changes, entity::invset &inval)
    {
        const auto F = obj.features() + (changes.features_assigned() ? changes.features() : featureset{});

        if (UserAccountSession.test(F))
        {
            inval(obj);
        }

        if (HaveRelation.test(F))
        {
            from(obj.subject()).filter(UserAccountSession).for_each(inval);
            from(changes.subject_assigned() ? changes.subject() : none).filter(UserAccountSession).for_each(inval);
            from(obj.object()).filter(UserAccountSession).for_each(inval);
            from(changes.object_assigned() ? changes.object() : none).filter(UserAccountSession).for_each(inval);
        }

        if (UserRelation.test(F))
        {
            from(obj).subject_of(HaveRelation).having_object(UserAccountSession).for_each(inval);
        }
    }

    static bool refresh(const entity &root, RoomBooking::UserSession::refresh_batch &changes)
    {
        if (!UserAccountSession.test(root))
        {
            return false;
        }

        const auto user = from(root).object_of(HaveRelation).having_subject(UserRelation).first();
        const auto sessionId = from(root).subject_of(HaveRelation).having_object(Identifier).first();
        const auto expiresAt = root.end();

        changes.User(user ? some(user->to<RoomBooking::User>()) : none);
        changes.SessionId(sessionId ? sessionId->string() : none);
        changes.ExpiresAt(expiresAt ? *expiresAt : datetime::max_value());

        return true;
    }
};